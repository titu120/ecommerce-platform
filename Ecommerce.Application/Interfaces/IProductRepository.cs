using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> SearchProductsAsync(string keyword);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
    Task<Product?> GetProductWithCategoryAsync(int id);
}