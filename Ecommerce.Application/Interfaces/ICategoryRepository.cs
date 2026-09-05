using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetCategoryWithProductsAsync(int id);
}