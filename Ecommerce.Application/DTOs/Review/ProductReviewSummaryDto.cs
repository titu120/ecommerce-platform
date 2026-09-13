namespace Ecommerce.Application.DTOs.Review
{
    public class ProductReviewSummaryDto
    {
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new();
    }
}