namespace EnterBridgePOC.Models
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public double PriceAtOrder { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
    }
}
