using KanbanAppApi.Dtos;
using KanbanAppApi.Filters;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanbanAppApi.Controllers
{
    [Authorize]
    [RequireUserId]
    [ApiController]
    [Route("api/boardTasks")]
    public class BoardTaskController : ControllerBase
    {
        private readonly IBoardTaskService _boardTaskService;

        public BoardTaskController(IBoardTaskService boardTaskService)
        {
            _boardTaskService = boardTaskService;
        }

        [HttpPost]
        public async Task<IResult> CreateBoardTask([FromBody] CreateBoardTaskRequestDto boardTaskRequest)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var createdBoardTaskService = await _boardTaskService.CreateBoardTaskAsync(userId, boardTaskRequest);

            return createdBoardTaskService.Succeeded 
                ? TypedResults.Ok(createdBoardTaskService)
                : TypedResults.BadRequest(createdBoardTaskService);
        }

        [HttpPut]
        public async Task<IResult> UpdateBoardTask([FromBody] UpdateBoardTaskRequestDto boardTaskRequest)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var updatedBoardTaskService = await _boardTaskService.UpdateBoardTaskAsync(userId, boardTaskRequest);

            return updatedBoardTaskService.Succeeded
               ? TypedResults.Ok(updatedBoardTaskService)
               : TypedResults.BadRequest(updatedBoardTaskService);
        }

        [HttpDelete("{taskId}")]
        public async Task<IResult> DeleteBoardTask([FromRoute] int taskId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var deletedBoardTaskService = await _boardTaskService.DeleteBoardTaskAsync(userId, taskId);

            return deletedBoardTaskService.Succeeded
               ? TypedResults.Ok(deletedBoardTaskService)
               : TypedResults.BadRequest(deletedBoardTaskService);
        }

        [HttpPut("reorder")]
        public async Task<IResult> ReorderBoardTask([FromBody] ReorderBoardTaskRequestDto boardTaskRequest)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var reorderedBoardTaskService = await _boardTaskService.ReorderBoardTaskAsync(userId, boardTaskRequest);

            return reorderedBoardTaskService.Succeeded
               ? TypedResults.Ok(reorderedBoardTaskService)
               : TypedResults.BadRequest(reorderedBoardTaskService);
        }

    }
}
