using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces
{
    public interface IWishlistRepository : IGenericRepository<Wishlist>
    {
        Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId);
        Task<Wishlist?> FindAsync(int userId, int productId);
    }
}