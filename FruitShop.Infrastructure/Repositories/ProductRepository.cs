using FruitShop.Application.Interfaces;
using FruitShop.Domain.Entities;
using FruitShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitShop.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;

        public ProductRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

        public async Task<Product> CreateAsync(Product product)
        {
            _appDbContext.Products.Add(product);
            await _appDbContext.SaveChangesAsync();
            return product;
        }

        public Task<Product?> GetProductByIdAsync(int id)
        {
            return _appDbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<List<Product>> GetProductsAsync()
        {
            return _appDbContext.Products.AsNoTracking().Take(100).ToListAsync();
        }
    }
}
