using FluentValidation;

namespace Enterprise.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Validator for CreateProductCommand using FluentValidation.
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(v => v.Sku)
            .NotEmpty().WithMessage("SKU code is required.")
            .MaximumLength(50).WithMessage("SKU code must not exceed 50 characters.");

        RuleFor(v => v.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(v => v.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency code must be exactly 3 characters (e.g., USD, TRY).");

        RuleFor(v => v.InitialStock)
            .GreaterThanOrEqualTo(0).WithMessage("Initial stock cannot be negative.");
    }
}
