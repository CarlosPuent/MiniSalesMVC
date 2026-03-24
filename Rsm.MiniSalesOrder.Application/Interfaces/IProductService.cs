using Rsm.MiniSalesOrder.Domain.Entities;

namespace Rsm.MiniSalesOrder.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateProductStockAsync(int productId, int quantity);
        Task<int> GetProductCountAsync();
    }
}