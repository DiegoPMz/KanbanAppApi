using FluentValidation;
using KanbanAppApi.Domain.TaskAggregate;

namespace KanbanAppApi.Features.Task.Shared;

public static class SubTaskValidations
{
    public static IRuleBuilderOptions<T, string?> ValidSubTaskDescription<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage(SubTaskErrors.DescriptionRequired.Description)
            .MaximumLength(250).WithMessage(SubTaskErrors.DescriptionTooLong(250).Description);
    }
}