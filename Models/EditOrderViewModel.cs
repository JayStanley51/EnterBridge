namespace EnterBridgePOC.Models
{
    public class EditOrderViewModel
    {
        public int OrderId { get; set; }
        public List<EditOrderItemViewModel> Items { get; set; } = [];
    }

    public class EditOrderItemViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public double PriceAtOrder { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool Remove { get; set; }
    }
}
