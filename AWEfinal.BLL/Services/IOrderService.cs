using AWEfinal.DAL.Models;

namespace AWEfinal.BLL.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems);
        Task<Order> UpdateOrderStatusAsync(int orderId, string status, string? trackingNumber = null);
        Task<bool> DeleteOrderAsync(int id);
    }
}

