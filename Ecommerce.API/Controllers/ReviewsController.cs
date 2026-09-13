using Ecommerce.Application.DTOs.Review;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ICurrentUserService _currentUserService;

        public ReviewsController(IReviewService reviewService, ICurrentUserService currentUserService)
        {
            _reviewService = reviewService;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
        {
            var userId = _currentUserService.UserId!.Value;
            var review = await _reviewService.AddReviewAsync(userId, dto);
            return Ok(review);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var summary = await _reviewService.GetProductReviewsAsync(productId);
            return Ok(summary);
        }
    }
}