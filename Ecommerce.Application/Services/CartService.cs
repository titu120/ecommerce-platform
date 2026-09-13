using Ecommerce.Application.DTOs.Cart;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using FluentValidation;

namespace Ecommerce.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AddToCartDto> _addValidator;
        private readonly IValidator<UpdateCartItemDto> _updateValidator;

        public CartService(
            IUnitOfWork unitOfWork,
            IValidator<AddToCartDto> addValidator,
            IValidator<UpdateCartItemDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            var validationResult = await _addValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(validationResult.Errors);
            }

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Product", dto.ProductId);
            }

            if (product.StockQuantity < dto.Quantity)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(new List<FluentValidation.Results.ValidationFailure>
                {
                    new("Quantity", $"Only {product.StockQuantity} units available in stock.")
                });
            }

            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + dto.Quantity;

                if (product.StockQuantity < newQuantity)
                {
                    throw new Ecommerce.Application.Exceptions.ValidationException(new List<FluentValidation.Results.ValidationFailure>
                    {
                        new("Quantity", $"Only {product.StockQuantity} units available in stock.")
                    });
                }

                existingItem.Quantity = newQuantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                await _unitOfWork.CartItems.AddAsync(newItem);
            }

            await _unitOfWork.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null)
            {
                return new CartDto { UserId = userId, Items = new List<CartItemDto>(), TotalPrice = 0, TotalItems = 0 };
            }

            return MapCartToDto(cart);
        }

        public async Task<CartDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(validationResult.Errors);
            }

            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Cart", userId);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("CartItem", cartItemId);
            }

            var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
            if (product != null && product.StockQuantity < dto.Quantity)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(new List<FluentValidation.Results.ValidationFailure>
                {
                    new("Quantity", $"Only {product.StockQuantity} units available in stock.")
                });
            }

            cartItem.Quantity = dto.Quantity;
            await _unitOfWork.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Cart", userId);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("CartItem", cartItemId);
            }

            _unitOfWork.CartItems.Delete(cartItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                return;
            }

            foreach (var item in cart.CartItems.ToList())
            {
                _unitOfWork.CartItems.Delete(item);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private static CartDto MapCartToDto(Cart cart)
        {
            var items = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ProductImageUrl = ci.Product.ImageUrl,
                UnitPrice = ci.Product.Price,
                Quantity = ci.Quantity,
                Subtotal = ci.Product.Price * ci.Quantity
            }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = items,
                TotalPrice = items.Sum(i => i.Subtotal),
                TotalItems = items.Sum(i => i.Quantity)
            };
        }
    }
}