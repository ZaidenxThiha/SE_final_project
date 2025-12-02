using System.Threading.Tasks;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using AWEfinal.DAL.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AWEfinal.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task Create_InvalidName_Throws()
        {
            var repo = new Mock<IProductRepository>();
            var svc = new ProductService(repo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => svc.CreateProductAsync(new Product { Name = "", Price = 10 }));
        }

        [Fact]
        public async Task Create_PriceZero_Throws()
        {
            var repo = new Mock<IProductRepository>();
            var svc = new ProductService(repo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => svc.CreateProductAsync(new Product { Name = "Test", Price = 0 }));
        }

        [Fact]
        public async Task Update_NotFound_Throws()
        {
            var repo = new Mock<IProductRepository>();
            repo.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(false);
            var svc = new ProductService(repo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => svc.UpdateProductAsync(new Product { Id = 99, Name = "Test", Price = 10 }));
        }

        [Fact]
        public async Task Update_Valid_Succeeds()
        {
            var repo = new Mock<IProductRepository>();
            repo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            repo.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

            var svc = new ProductService(repo.Object);
            var updated = await svc.UpdateProductAsync(new Product { Id = 1, Name = "Phone", Price = 999 });

            updated.Price.Should().Be(999);
            updated.Name.Should().Be("Phone");
        }
    }
}
