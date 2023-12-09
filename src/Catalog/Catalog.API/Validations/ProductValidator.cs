using Catalog.API.Models;
using FluentValidation;

namespace Catalog.API.Validations;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .MinimumLength(5)
            .MaximumLength(80)
            .NotEmpty();

        RuleFor(p => p.Description)
            .MinimumLength(1)
            .MaximumLength(255)
            .NotEmpty()
            .When(p => p.Description is not null);

        RuleFor(p => p.Price)
            .GreaterThan(0);
    }
}