using Ecommerce.Application.DTOs.Review;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using FluentValidation;

namespace Ecommerce.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateReviewDto> _validator;

        public ReviewService(IUnitOfWork unitOfWork, IValidator<CreateReviewDto> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<ReviewDto> AddReviewAsync(int userId, CreateReviewDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(validationResult.Errors);
            }

            // Order আসলে এই User এর কিনা, এবং Delivered কিনা, এবং এই Product সেই Order এ ছিল কিনা — চেক করা
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(dto.OrderId);
            if (order == null || order.UserId != userId)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Order", dto.OrderId);
            }

            if (order.Status != OrderStatus.Delivered)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(
                    new List<FluentValidation.Results.ValidationFailure>
                    {
                        new("Order", "You can only review products from delivered orders.")
                    });
            }

            var productInOrder = order.OrderItems.Any(oi => oi.ProductId == dto.ProductId);
            if (!productInOrder)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(
                    new List<FluentValidation.Results.ValidationFailure>
                    {
                        new("ProductId", "This product was not part of the specified order.")
                    });
            }

            var alreadyReviewed = await _unitOfWork.Reviews.HasUserReviewedOrderAsync(userId, dto.OrderId, dto.ProductId);
            if (alreadyReviewed)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(
                    new List<FluentValidation.Results.ValidationFailure>
                    {
                        new("Review", "You have already reviewed this product for this order.")
                    });
            }

            var review = new Review
            {
                ProductId = dto.ProductId,
                OrderId = dto.OrderId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            return new ReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                UserName = user?.FullName ?? string.Empty,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<ProductReviewSummaryDto> GetProductReviewsAsync(int productId)
        {
            var reviews = (await _unitOfWork.Reviews.GetReviewsByProductAsync(productId)).ToList();

            var reviewDtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                UserName = r.User.FullName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new ProductReviewSummaryDto
            {
                AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 2) : 0,
                ReviewCount = reviews.Count,
                Reviews = reviewDtos
            };
        }
    }
}