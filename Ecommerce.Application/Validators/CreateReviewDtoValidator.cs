using Ecommerce.Application.DTOs.Review;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.OrderId).GreaterThan(0);
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.Comment).MaximumLength(1000);
        }
    }
}