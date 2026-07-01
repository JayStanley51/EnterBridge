using EnterBridgePOC.Models;
using System.Globalization;

namespace EnterBridgePOC.Services
{
    public class OrderStore : IOrderStore
    {
        private readonly string _csvPath;
        private readonly List<Order> _orders = [];
        private int _nextId = 1;
        private readonly Lock _lock = new();

        private const string Header = "OrderId,PlacedAt,Role,ProductId,Name,SKU,Quantity,PriceAtOrder,UnitOfMeasure,Status,ReviewedAt";

        public OrderStore(IWebHostEnvironment env)
        {
            _csvPath = Path.Combine(env.ContentRootPath, "Data", "orders.csv");
            Load();
        }

        public Order Save(Order order)
        {
            lock (_lock)
            {
                order.Id = _nextId++;
                _orders.Add(order);
                Rewrite();
            }
            return order;
        }

        public bool UpdateItems(int orderId, List<OrderItem> items)
        {
            lock (_lock)
            {
                var order = _orders.FirstOrDefault(o => o.Id == orderId);
                if (order is null) return false;
                order.Items = items;
                Rewrite();
                return true;
            }
        }

        public bool UpdateStatus(int orderId, ApprovalStatus status)
        {
            lock (_lock)
            {
                var order = _orders.FirstOrDefault(o => o.Id == orderId);
                if (order is null) return false;
                order.Status = status;
                order.ReviewedAt = DateTime.Now;
                Rewrite();
                return true;
            }
        }

        public IReadOnlyList<Order> GetAll()
        {
            lock (_lock) return _orders.AsReadOnly();
        }

        private void Load()
        {
            if (!File.Exists(_csvPath)) return;

            var lines = File.ReadAllLines(_csvPath);
            var byOrder = new Dictionary<int, Order>();

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var cols = line.Split(',');
                if (cols.Length < 7) continue;

                var id = int.Parse(cols[0]);
                if (!byOrder.TryGetValue(id, out var order))
                {
                    order = new Order
                    {
                        Id = id,
                        PlacedAt = DateTime.Parse(cols[1], CultureInfo.InvariantCulture),
                        Role = cols[2],
                        Status = cols.Length > 9 && Enum.TryParse<ApprovalStatus>(cols[9], out var s) ? s : ApprovalStatus.Pending,
                        ReviewedAt = cols.Length > 10 && !string.IsNullOrEmpty(cols[10])
                            ? DateTime.Parse(cols[10], CultureInfo.InvariantCulture)
                            : null
                    };
                    byOrder[id] = order;
                }

                order.Items.Add(new OrderItem
                {
                    ProductId = int.Parse(cols[3]),
                    Name = cols[4],
                    Sku = cols[5],
                    Quantity = int.Parse(cols[6]),
                    PriceAtOrder = cols.Length > 7 ? double.Parse(cols[7], CultureInfo.InvariantCulture) : 0,
                    UnitOfMeasure = cols.Length > 8 ? cols[8] : string.Empty
                });
            }

            _orders.AddRange(byOrder.Values.OrderBy(o => o.Id));
            _nextId = _orders.Count > 0 ? _orders.Max(o => o.Id) + 1 : 1;
        }

        private void Rewrite()
        {
            using var writer = new StreamWriter(_csvPath, append: false);
            writer.WriteLine(Header);
            foreach (var order in _orders)
            {
                foreach (var item in order.Items)
                {
                    writer.WriteLine(string.Join(",",
                        order.Id,
                        order.PlacedAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        order.Role,
                        item.ProductId,
                        item.Name,
                        item.Sku,
                        item.Quantity,
                        item.PriceAtOrder.ToString(CultureInfo.InvariantCulture),
                        item.UnitOfMeasure,
                        order.Status,
                        order.ReviewedAt?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty));
                }
            }
        }
    }
}
