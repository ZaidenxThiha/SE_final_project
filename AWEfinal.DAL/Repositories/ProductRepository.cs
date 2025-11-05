using AWEfinal.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AWEfinal.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AWEfinalDbContext _context;

        public ProductRepository(AWEfinalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            // Check if entity is already being tracked by the context
            var trackedEntity = _context.Products.Local.FirstOrDefault(p => p.Id == product.Id);
            
            if (trackedEntity != null)
            {
                // Entity is already tracked, update its properties
                _context.Entry(trackedEntity).CurrentValues.SetValues(product);
                await _context.SaveChangesAsync();
                return trackedEntity;
            }
            else
            {
                // Check if entity exists in database but not tracked
                var existingEntity = await _context.Products.FindAsync(product.Id);
                if (existingEntity != null)
                {
                    // Entity exists but not tracked, update its properties
                    _context.Entry(existingEntity).CurrentValues.SetValues(product);
                    await _context.SaveChangesAsync();
                    return existingEntity;
                }
                else
                {
                    // New entity or not found, use Update
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    return product;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }
    }
}

