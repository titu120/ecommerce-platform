using Ecommerce.Application.DTOs.Cart;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICurrentUserService _currentUserService;

        public CartsController(ICartService cartService, ICurrentUserService currentUserService)
        {
            _cartService = cartService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = _currentUserService.UserId!.Value;
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = _currentUserService.UserId!.Value;
            var cart = await _cartService.AddToCartAsync(userId, dto);
            return Ok(cart);
        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            var userId = _currentUserService.UserId!.Value;
            var cart = await _cartService.UpdateCartItemAsync(userId, cartItemId, dto);
            return Ok(cart);
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = _currentUserService.UserId!.Value;
            await _cartService.RemoveFromCartAsync(userId, cartItemId);
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = _currentUserService.UserId!.Value;
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}