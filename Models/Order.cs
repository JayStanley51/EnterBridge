namespace EnterBridgePOC.Models
{
    public enum ApprovalStatus { Pending, Approved, Rejected }

    public class Order
    {
        public int Id { get; set; }
        public DateTime PlacedAt { get; set; } = DateTime.Now;
        public string Role { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = [];
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        public DateTime? ReviewedAt { get; set; }
    }
}
