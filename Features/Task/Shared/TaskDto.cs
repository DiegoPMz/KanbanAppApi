using KanbanAppApi.Domain.TaskAggregate;
using TaskModel = KanbanAppApi.Domain.TaskAggregate.Task;

namespace KanbanAppApi.Features.Task.Shared;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string Description,
    PriorityType Priority,
    bool IsCompleted,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid ColumnId
)
{
    public static TaskDto FromEntity(TaskModel e) => new TaskDto(
        e.Id,
        e.Title,
        e.Description,
        e.Priority,
        e.IsCompleted,
        e.CreatedAt,
        e.UpdatedAt,
        e.ColumnId
    );
};