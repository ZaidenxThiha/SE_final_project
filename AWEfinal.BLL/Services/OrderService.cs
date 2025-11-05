using AWEfinal.DAL.Models;
using AWEfinal.DAL.Repositories;

namespace AWEfinal.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order?> GetOrderByOrderNumberAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                return null;

            return await _orderRepository.GetByOrderNumberAsync(orderNumber);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            if (userId <= 0)
                return Enumerable.Empty<Order>();

            return await _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (orderItems == null || !orderItems.Any())
                throw new ArgumentException("Order must have at least one item", nameof(orderItems));

            // Generate order number
            order.OrderNumber = GenerateOrderNumber();
            order.InvoiceNumber = GenerateInvoiceNumber();

            // Calculate total
            order.Total = orderItems.Sum(oi => oi.Subtotal);
            order.Status = "pending";
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            // Validate products exist
            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");

                item.Price = product.Price;
                item.Subtotal = item.Price * item.Quantity;
                item.Order = order;
            }

            order.OrderItems = orderItems;
            return await _orderRepository.CreateAsync(order);
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, string status, string? trackingNumber = null)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID", nameof(orderId));

            var validStatuses = new[] { "pending", "paid", "packaging", "shipped", "delivered", "cancelled" };
            if (!validStatuses.Contains(status.ToLower()))
                throw new ArgumentException("Invalid order status", nameof(status));

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found", nameof(orderId));

            order.Status = status.ToLower();
            order.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(trackingNumber))
                order.TrackingNumber = trackingNumber;

            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _orderRepository.DeleteAsync(id);
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }
    }
}

