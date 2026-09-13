using Ecommerce.Application.DTOs.Cart;

namespace Ecommerce.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto);
        Task RemoveFromCartAsync(int userId, int cartItemId);
        Task ClearCartAsync(int userId);
    }
}