using ErrorOr;
using KanbanAppApi.Domain.TaskAggregate;

namespace KanbanAppApi.Features.Task.Shared;

public record SubTaskDto(string Description, bool IsCompleted, Guid Id)
{
    public static SubTaskDto FromEntity(SubTask e) => new SubTaskDto(e.Description, e.IsCompleted, e.Id);
}