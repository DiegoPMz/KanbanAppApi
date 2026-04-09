using ErrorOr;

namespace KanbanAppApi.Common.Domain.TaskAggregate;

public static class TaskErrors
{
    public static Error NotFound(string identifier) => Error.NotFound(
        code: "TASK_NOT_FOUND",
        description: $"Task with identifier '{identifier}' could not be found.");

    public static Error TitleRequired = Error.Validation(
        code: "TASK_TITLE_REQUIRED",
        description: "The task title cannot be empty or consist only of white spaces.");

    public static Error TitleTooLong(int max) => Error.Validation(
        code: "TASK_TITLE_TOO_LONG",
        description: $"The title cannot be longer than {max} characters.");

    public static Error DescriptionTooLong(int max) => Error.Validation(
        code: "TASK_DESCRIPTION_TOO_LONG",
        description: $"The description cannot be longer than {max} characters.");

    public static Error InvalidPriority = Error.Validation(
        code: "TASK_INVALID_PRIORITY",
        description: "The provided priority level is not valid.");
    
    public static Error MaxSubTasksReached(int limit) => Error.Validation(
        code: "TASK_SUBTASK_LIMIT_REACHED",
        description: $"The task has reached the maximum allowed limit of {limit} subtasks.");
    
    public static Error SubTaskNotFound(string id) => Error.NotFound(
        code: "TASK_SUBTASK_NOT_FOUND",
        description: $"SubTask with identifier '{id}' could not be found in this task.");
}