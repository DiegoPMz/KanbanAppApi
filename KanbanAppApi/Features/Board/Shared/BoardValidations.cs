using FluentValidation;

namespace KanbanAppApi.Features.Board.Shared;

public static class BoardValidations
{
    public static IRuleBuilderOptions<T, string?> ValidBoardName<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Board name is required.")
            .MinimumLength(1).WithMessage("Name must be at least 1 character long.")
            .MaximumLength(250).WithMessage("Name cannot exceed 250 characters.");
    }
}