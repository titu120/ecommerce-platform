using Ecommerce.Application.DTOs.Cart;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
    {
        public UpdateCartItemDtoValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}