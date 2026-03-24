using Microsoft.EntityFrameworkCore;
using Rsm.MiniSalesOrder.Application.Interfaces;
using Rsm.MiniSalesOrder.Domain.Entities;
using Rsm.MiniSalesOrder.Infrastructure.Data;

namespace Rsm.MiniSalesOrder.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct is null)
            {
                return null;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.Stock = product.Stock;
            existingProduct.IsActive = product.IsActive;

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return false;
            }

            var isUsed = await _context.SalesOrderItems.AnyAsync(i => i.ProductId == id);
            if (isUsed)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateProductStockAsync(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return false;
            }

            var product = await _context.Products.FindAsync(productId);
            if (product is null)
            {
                return false;
            }

            if (product.Stock < quantity)
            {
                return false;
            }

            product.Stock -= quantity;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }
    }
}