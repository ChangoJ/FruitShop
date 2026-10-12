using FruitShop.Application.Interfaces;
using FruitShop.Domain.Entities;
using FruitShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FruitShop.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IDistributedCache _cache;

        private static readonly DistributedCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        public ProductRepository(AppDbContext appDbContext, IDistributedCache cache)
        {
            _appDbContext = appDbContext;
            _cache = cache;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            const string key = "products:all";
            
            var cached = await _cache.GetStringAsync(key);
            if (cached is not null)
                return JsonSerializer.Deserialize<List<Product>>(cached)!;

            var products = await _appDbContext.Products.AsNoTracking().Take(100).ToListAsync();
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(products), _cacheOptions);
            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var key = $"products:{id}";

            var cached = await _cache.GetStringAsync(key);
            if (cached is not null)
                return JsonSerializer.Deserialize<Product>(cached);

            var product = await _appDbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product is not null)
                await _cache.SetStringAsync(key, JsonSerializer.Serialize(product), _cacheOptions);
            return product;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _appDbContext.Products.Add(product);
            await _appDbContext.SaveChangesAsync();
            
            await _cache.RemoveAsync("products:all");
            return product;
        }
    }
}
