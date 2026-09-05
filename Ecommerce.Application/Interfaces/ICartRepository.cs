using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetCartByUserIdAsync(int userId);
    Task<Cart?> GetCartWithItemsAsync(int userId);
}