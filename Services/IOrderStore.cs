using EnterBridgePOC.Models;

namespace EnterBridgePOC.Services
{
    public interface IOrderStore
    {
        Order Save(Order order);
        IReadOnlyList<Order> GetAll();
        bool UpdateStatus(int orderId, ApprovalStatus status);
        bool UpdateItems(int orderId, List<OrderItem> items);
    }
}
