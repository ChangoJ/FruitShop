using FruitShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FruitShop.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {        

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(200).IsRequired();
                e.Property(p => p.Category).HasMaxLength(100).IsRequired();
                e.Property(p => p.Price).HasPrecision(10, 2);
                e.HasIndex(p => p.Category)
                    .IncludeProperties(p => new { p.Name, p.Price });
            });

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.UnitPrice).HasPrecision(10, 2);
        }
    }
}
