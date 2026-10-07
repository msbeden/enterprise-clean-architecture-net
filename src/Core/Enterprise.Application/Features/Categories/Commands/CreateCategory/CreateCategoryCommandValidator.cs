using FluentValidation;

namespace Enterprise.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// Validator for CreateCategoryCommand using FluentValidation.
/// </summary>
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(150).WithMessage("Category name must not exceed 150 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(v => v.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater.");
    }
}
