using FluentValidation;
using KanbanAppApi.Domain.TaskAggregate;

namespace KanbanAppApi.Features.Task.Shared;

public static class TaskValidations
{
    public static IRuleBuilderOptions<T, string?> ValidTaskTitle<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage(TaskErrors.TitleRequired.Description)
            .MaximumLength(250).WithMessage(TaskErrors.TitleTooLong(250).Description);
    }

    public static IRuleBuilderOptions<T, string?> ValidTaskDescription<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(1000).WithMessage(TaskErrors.DescriptionTooLong(1000).Description);
    }

    public static IRuleBuilderOptions<T, TProperty> ValidTaskPriority<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder
            .IsInEnum().WithMessage(TaskErrors.InvalidPriority.Description);
    }

    public static IRuleBuilderOptions<T, bool?> ValidTaskStatus<T>(this IRuleBuilder<T, bool?> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage("The completion status must be specified (true/false).");
    }
}