using AWEfinal.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AWEfinal.DAL
{
    public class AWEfinalDbContext : DbContext
    {
        public AWEfinalDbContext(DbContextOptions<AWEfinalDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("customer");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Category);
                entity.Property(e => e.InStock).HasDefaultValue(true);
                entity.Property(e => e.Rating).HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.Status).HasDefaultValue("pending");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // OrderItem configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.ProductId);
            });
        }
    }
}

