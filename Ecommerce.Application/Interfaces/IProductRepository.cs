using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(string keyword);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
        Task<Product?> GetProductWithCategoryAsync(int id);
        Task<IEnumerable<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, string? sortBy, bool isDescending);
        Task<int> GetTotalProductCountAsync();
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}