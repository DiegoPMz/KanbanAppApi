using KanbanAppApi.Data;
using KanbanAppApi.Domain.TaskAggregate;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public sealed class SyncBoardOnTaskDeletedHandler(ApplicationContextDb context) 
    : INotificationHandler<TaskDeletedEvent>
{
    public async ValueTask Handle(TaskDeletedEvent notification, CancellationToken ct)
    {
        var column = await context.Columns
            .FirstOrDefaultAsync(c => c.Id == notification.ColumnId, ct);
        
        if (column is null) return;
        
        column.RemoveTask(notification.TaskId);
        await context.SaveChangesAsync(ct);
    }
}