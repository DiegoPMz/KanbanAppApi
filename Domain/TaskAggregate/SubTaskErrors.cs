using ErrorOr;

namespace KanbanAppApi.Domain.TaskAggregate;

public static class SubTaskErrors
{
    public static Error DescriptionTooLong(int max) => Error.Validation(
        code: "SUBTASK_DESCRIPTION_TOO_LONG",
        description: $"The description cannot be longer than {max} characters.");
    
    public static Error DescriptionRequired = Error.Validation(
        code: "SUBTASK_DESCRIPTION_REQUIRED",
        description: "The subtask description cannot be empty or consist only of white spaces.");
}