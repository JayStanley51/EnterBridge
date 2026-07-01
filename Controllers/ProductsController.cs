using EnterBridgePOC.Models;
using EnterBridgePOC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterBridgePOC.Controllers
{
    public class ProductsController(IEnterBridgeApiService api) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var productsTask = api.GetProductsAsync(
                pageNumber: 1, pageSize: 1000,
                name: null, description: null, sku: null,
                category: null, sortBy: null, sortDirection: null);

            var pricesTask = api.GetPricesAsync(
                pageNumber: 1, pageSize: 1000,
                productId: null, startDate: null, endDate: null,
                minAmount: null, maxAmount: null,
                unitOfMeasure: null, sortBy: null, sortDirection: null);

            await Task.WhenAll(productsTask, pricesTask);

            var pricesByProduct = pricesTask.Result.Items
                .GroupBy(p => p.ProductId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.DateTime).ToList());

            var viewModel = productsTask.Result.Items
                .Select(p => new ProductWithPricesViewModel
                {
                    Product = p,
                    Prices = pricesByProduct.TryGetValue(p.Id, out var prices) ? prices : []
                })
                .ToList();

            return View(viewModel);
        }
    }
}
