using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using AWEfinal.DAL.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AWEfinal.Tests
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrder_RepricesItemsAndSetsTotal()
        {
            var product = new Product { Id = 1, Price = 100m, StockQuantity = 10, InStock = true };

            var productRepo = new Mock<IProductRepository>();
            productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var orderRepo = new Mock<IOrderRepository>();
            orderRepo.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

            var service = new OrderService(orderRepo.Object, productRepo.Object);

            var order = new Order { UserId = 1 };
            var items = new List<OrderItem> { new() { ProductId = 1, Quantity = 2, Subtotal = 1m } }; // stale subtotal

            var created = await service.CreateOrderAsync(order, items);

            created.Total.Should().Be(200m);
            created.OrderItems.Single().Price.Should().Be(100m);
        }

        [Fact]
        public async Task CreateOrder_NoItems_Throws()
        {
            var productRepo = new Mock<IProductRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            var service = new OrderService(orderRepo.Object, productRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(new Order { UserId = 1 }, new List<OrderItem>()));
        }

        [Theory]
        [InlineData("paid")]
        [InlineData("packaging")]
        [InlineData("shipped")]
        [InlineData("delivered")]
        public async Task UpdateStatus_DecrementsStockOnce_ForPaidLikeStatuses(string status)
        {
            var product = new Product { Id = 1, Price = 50m, StockQuantity = 5, InStock = true };
            var productRepo = new Mock<IProductRepository>();
            productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            productRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

            var order = new Order
            {
                Id = 1,
                UserId = 1,
                Status = "pending",
                Total = 100m,
                OrderItems = new List<OrderItem> { new() { ProductId = 1, Quantity = 3 } },
                InventoryAdjusted = false
            };
            var orderRepo = new Mock<IOrderRepository>();
            orderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
            orderRepo.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

            var service = new OrderService(orderRepo.Object, productRepo.Object);
            await service.UpdateOrderStatusAsync(1, status);

            product.StockQuantity.Should().Be(2);
            product.InStock.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateStatus_Cancelled_RestoresStock_WhenPreviouslyAdjusted()
        {
            var product = new Product { Id = 1, Price = 50m, StockQuantity = 2, InStock = true };
            var productRepo = new Mock<IProductRepository>();
            productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            productRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

            var order = new Order
            {
                Id = 1,
                UserId = 1,
                Status = "paid",
                Total = 100m,
                OrderItems = new List<OrderItem> { new() { ProductId = 1, Quantity = 2 } },
                InventoryAdjusted = true
            };
            var orderRepo = new Mock<IOrderRepository>();
            orderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
            orderRepo.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

            var service = new OrderService(orderRepo.Object, productRepo.Object);
            await service.UpdateOrderStatusAsync(1, "cancelled");

            product.StockQuantity.Should().Be(4);
            product.InStock.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateStatus_InvalidStatus_Throws()
        {
            var productRepo = new Mock<IProductRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Order { Id = 1, OrderItems = new List<OrderItem>() });

            var service = new OrderService(orderRepo.Object, productRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateOrderStatusAsync(1, "badstatus"));
        }
    }
}
