using Ecommerce.Application.DTOs.Wishlist;

namespace Ecommerce.Application.Interfaces
{
    public interface IWishlistService
    {
        Task<WishlistDto> AddToWishlistAsync(int userId, int productId);
        Task RemoveFromWishlistAsync(int userId, int productId);
        Task<IEnumerable<WishlistDto>> GetMyWishlistAsync(int userId);
    }
}