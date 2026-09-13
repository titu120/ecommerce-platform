using Ecommerce.Application.DTOs.Review;

namespace Ecommerce.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(int userId, CreateReviewDto dto);
        Task<ProductReviewSummaryDto> GetProductReviewsAsync(int productId);
    }
}