using KanbanAppApi.Data;
using KanbanAppApi.Domain.TaskAggregate;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public sealed class OnTaskCreatedUpdateBoardHandler(ApplicationContextDb context) 
    : INotificationHandler<TaskCreatedEvent>
{
    public async ValueTask Handle(TaskCreatedEvent notification, CancellationToken ct)
    {
        var column = await context.Columns
            .FirstOrDefaultAsync(c => c.Id == notification.ColumnId, ct);

        if (column is null) return;
        
        column.AddTask(notification.TaskId);
        await context.SaveChangesAsync(ct);
    }
}