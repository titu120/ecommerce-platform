using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ICurrentUserService _currentUserService;

        public WishlistController(IWishlistService wishlistService, ICurrentUserService currentUserService)
        {
            _wishlistService = wishlistService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = _currentUserService.UserId!.Value;
            var wishlist = await _wishlistService.GetMyWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = _currentUserService.UserId!.Value;
            var result = await _wishlistService.AddToWishlistAsync(userId, productId);
            return Ok(result);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = _currentUserService.UserId!.Value;
            await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            return NoContent();
        }
    }
}