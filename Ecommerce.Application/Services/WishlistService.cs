using Ecommerce.Application.DTOs.Wishlist;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WishlistDto> AddToWishlistAsync(int userId, int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Product", productId);
            }

            var existing = await _unitOfWork.Wishlists.FindAsync(userId, productId);
            if (existing != null)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(
                    new List<FluentValidation.Results.ValidationFailure>
                    {
                        new("Product", "This product is already in your wishlist.")
                    });
            }

            var wishlist = new Wishlist { UserId = userId, ProductId = productId };
            await _unitOfWork.Wishlists.AddAsync(wishlist);
            await _unitOfWork.SaveChangesAsync();

            return new WishlistDto
            {
                Id = wishlist.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };
        }

        public async Task RemoveFromWishlistAsync(int userId, int productId)
        {
            var wishlist = await _unitOfWork.Wishlists.FindAsync(userId, productId);
            if (wishlist == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Wishlist item", productId);
            }

            _unitOfWork.Wishlists.Delete(wishlist);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<WishlistDto>> GetMyWishlistAsync(int userId)
        {
            var wishlists = await _unitOfWork.Wishlists.GetByUserIdAsync(userId);

            return wishlists.Select(w => new WishlistDto
            {
                Id = w.Id,
                ProductId = w.Product.Id,
                ProductName = w.Product.Name,
                Price = w.Product.Price,
                ImageUrl = w.Product.ImageUrl
            });
        }
    }
}