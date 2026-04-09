using FluentValidation;

namespace KanbanAppApi.Features.Board.Shared;

public static class ColumnValidations
{
    public static IRuleBuilderOptions<T, string?> ValidColumnName<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Column name is required.")
            .MinimumLength(1).WithMessage("Name must be at least 1 character long.")
            .MaximumLength(250).WithMessage("Name cannot exceed 250 characters.");
    }

    public static IRuleBuilderOptions<T, string?> ValidColumnColor<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Color must be a valid hexadecimal format (e.g., #FFFFFF).");
    }

    public static IRuleBuilderOptions<T, int> ValidColumnOrder<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(1).WithMessage("Order cannot be a negative value.");
    }
}