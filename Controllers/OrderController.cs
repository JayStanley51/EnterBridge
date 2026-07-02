using EnterBridgePOC.Models;
using EnterBridgePOC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EnterBridgePOC.Controllers
{
    public class OrderController(IEnterBridgeApiService api, IOrderStore orderStore) : Controller
    {
        private const string CartKey = "Cart";

        private List<OrderItem> GetCart() =>
            JsonSerializer.Deserialize<List<OrderItem>>(
                HttpContext.Session.GetString(CartKey) ?? "[]")!;

        private void SaveCart(List<OrderItem> cart) =>
            HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));

        public async Task<IActionResult> Index()
        {
            var result = await api.GetProductsAsync(
                pageNumber: 1, pageSize: 1000,
                name: null, description: null, sku: null,
                category: null, sortBy: null, sortDirection: null);

            ViewData["OrderCount"] = GetCart().Sum(i => i.Quantity);
            return View(result.Items);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int productId, string name, string sku, int quantity = 1)
        {
            if (quantity < 1) quantity = 1;
            var cart = GetCart();
            var existing = cart.FirstOrDefault(i => i.ProductId == productId);
            if (existing is not null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                var prices = await api.GetPricesAsync(
                    pageNumber: 1, pageSize: 1,
                    productId: productId,
                    startDate: null, endDate: null,
                    minAmount: null, maxAmount: null,
                    unitOfMeasure: null,
                    sortBy: Models.Api.PriceSortBy.DateTime,
                    sortDirection: Models.Api.SortDirection.Desc);

                var latest = prices.Items.FirstOrDefault();
                cart.Add(new OrderItem
                {
                    ProductId = productId,
                    Name = name,
                    Sku = sku,
                    Quantity = quantity,
                    PriceAtOrder = latest?.Amount ?? 0,
                    UnitOfMeasure = latest?.UnitOfMeasure.ToString() ?? string.Empty
                });
            }

            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Cart() =>
            View(GetCart());

        [HttpPost]
        public IActionResult UpdateCart(Models.EditOrderViewModel vm, string action)
        {
            var items = vm.Items
                .Where(i => !i.Remove && i.Quantity > 0)
                .Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Name = i.Name,
                    Sku = i.Sku,
                    PriceAtOrder = i.PriceAtOrder,
                    UnitOfMeasure = i.UnitOfMeasure,
                    Quantity = i.Quantity
                }).ToList();

            SaveCart(items);

            return action == "place" ? SubmitOrder() : RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public IActionResult PlaceOrder() => SubmitOrder();

        private IActionResult SubmitOrder()
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction(nameof(Index));

            var order = new Order
            {
                Role = HttpContext.Session.GetString("Role") ?? "Unknown",
                Items = cart
            };

            orderStore.Save(order);
            HttpContext.Session.Remove(CartKey);

            TempData["ConfirmedOrderId"] = order.Id;
            return RedirectToAction(nameof(Confirmation));
        }

        public IActionResult History()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;

            var orders = orderStore.GetAll()
                .Where(o => role == "Foreman" || o.Role == role)
                .OrderByDescending(o => o.PlacedAt)
                .ToList();

            return View(orders);
        }

        [HttpPost]
        public IActionResult Reorder(int orderId)
        {
            var order = orderStore.GetAll().FirstOrDefault(o => o.Id == orderId);
            if (order is null) return NotFound();

            var cart = order.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Name = i.Name,
                Sku = i.Sku,
                Quantity = i.Quantity,
                PriceAtOrder = i.PriceAtOrder,
                UnitOfMeasure = i.UnitOfMeasure
            }).ToList();

            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditOrder(int orderId)
        {
            if (HttpContext.Session.GetString("Role") != "Foreman")
                return Forbid();

            var order = orderStore.GetAll().FirstOrDefault(o => o.Id == orderId);
            if (order is null) return NotFound();

            var vm = new Models.EditOrderViewModel
            {
                OrderId = order.Id,
                Items = order.Items.Select(i => new Models.EditOrderItemViewModel
                {
                    ProductId = i.ProductId,
                    Name = i.Name,
                    Sku = i.Sku,
                    PriceAtOrder = i.PriceAtOrder,
                    UnitOfMeasure = i.UnitOfMeasure,
                    Quantity = i.Quantity
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult SaveEdit(Models.EditOrderViewModel vm)
        {
            if (HttpContext.Session.GetString("Role") != "Foreman")
                return Forbid();

            var items = vm.Items
                .Where(i => !i.Remove && i.Quantity > 0)
                .Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Name = i.Name,
                    Sku = i.Sku,
                    PriceAtOrder = i.PriceAtOrder,
                    UnitOfMeasure = i.UnitOfMeasure,
                    Quantity = i.Quantity
                }).ToList();

            orderStore.UpdateItems(vm.OrderId, items);
            return RedirectToAction(nameof(History));
        }

        [HttpPost]
        public IActionResult Approve(int orderId)
        {
            if (HttpContext.Session.GetString("Role") != "Foreman")
                return Forbid();
            orderStore.UpdateStatus(orderId, Models.ApprovalStatus.Approved);
            return RedirectToAction(nameof(History));
        }

        [HttpPost]
        public IActionResult Reject(int orderId)
        {
            if (HttpContext.Session.GetString("Role") != "Foreman")
                return Forbid();
            orderStore.UpdateStatus(orderId, Models.ApprovalStatus.Rejected);
            return RedirectToAction(nameof(History));
        }

        public IActionResult Confirmation()
        {
            if (TempData["ConfirmedOrderId"] is not int id)
                return RedirectToAction(nameof(Index));

            var order = orderStore.GetAll().FirstOrDefault(o => o.Id == id);
            return order is null ? RedirectToAction(nameof(Index)) : View(order);
        }
    }
}
