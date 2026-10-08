using FluentValidation;

namespace Enterprise.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Validator for CreateOrderCommand using FluentValidation.
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(v => v.Street)
            .NotEmpty().WithMessage("Street address is required.")
            .MaximumLength(200).WithMessage("Street address must not exceed 200 characters.");

        RuleFor(v => v.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(v => v.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(v => v.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(v => v.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("Product ID is required.");
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}
