using AWEfinal.DAL.Models;
using AWEfinal.DAL.Repositories;

namespace AWEfinal.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return await _productRepository.GetAllAsync();

            return await _productRepository.GetByCategoryAsync(category);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name is required", nameof(product));

            if (product.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero", nameof(product));

            product.CreatedAt = DateTime.UtcNow;
            return await _productRepository.CreateAsync(product);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (!await _productRepository.ExistsAsync(product.Id))
                throw new ArgumentException("Product not found", nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name is required", nameof(product));

            if (product.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero", nameof(product));

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _productRepository.DeleteAsync(id);
        }
    }
}

