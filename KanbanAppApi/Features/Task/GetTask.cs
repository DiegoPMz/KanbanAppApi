using System.Security.Claims;
using ErrorOr;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Common.Persistence;
using KanbanAppApi.Features.Task.Shared;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Task;

public sealed class GetTask
{
    public record struct GetTaskQuery(Guid UserId, Guid TaskId):IQuery<ErrorOr<TaskDto>>;

    public sealed class GetTaskHandler(ApplicationContextDb context) 
        : IQueryHandler<GetTaskQuery,ErrorOr<TaskDto>>
    {
        public async ValueTask<ErrorOr<TaskDto>> Handle(GetTaskQuery query, CancellationToken ct)
        {
            var task = await context.Tasks
                .AsNoTracking()
                .Where(t => t.Id == query.TaskId)
                .Where(t => context.Boards.Any(b =>
                    b.UserId == query.UserId &&
                    b.Columns.Any(c => c.Id == t.ColumnId)))
                .FirstOrDefaultAsync(ct);
            
            if (task is null) return Error.NotFound(); 
    
            return TaskDto.FromEntity(task);
        }
    }
    
    public static class GetTaskEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("api/tasks/{taskId:guid}", async (
                ClaimsPrincipal user,
                CancellationToken ct, 
                Guid taskId,
                IMediator mediator
                ) =>
                
            {
                if (user.GetUserId() is not { } userId) 
                    return ApiErrorHandler.Problem(Error.Unauthorized());

                var query = new GetTaskQuery(userId, taskId);
                var result = await mediator.Send(query, ct);

                return result.Match(
                    Results.Ok,
                    errors => ApiErrorHandler.Problem(errors.First())
                );
            }).RequireAuthorization();
        }
    }
    
}