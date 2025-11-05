using KanbanAppApi.Dtos;
using KanbanAppApi.Errors;
using KanbanAppApi.Filters;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanbanAppApi.Controllers;

[Authorize]
[RequireUserId]
[ApiController]
[Route("api/boardTasks")]
public class BoardTaskController : ControllerBase
{
    private readonly IBoardTaskService _boardTaskService;
    public BoardTaskController(IBoardTaskService boardTaskService) => _boardTaskService = boardTaskService;

    [HttpPost]
    [ProducesResponseType(typeof(BoarTaskDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBoardTask([FromBody] CreateBoardTaskRequestDto boardTaskRequest)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var createdBoardTaskResult = await _boardTaskService.CreateAsync(userId, boardTaskRequest);
        
        return createdBoardTaskResult.IsSuccess 
            ? Ok(createdBoardTaskResult.Value)
            : Problem(
                title: createdBoardTaskResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: createdBoardTaskResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpPut]
    [ProducesResponseType(typeof(UpdateBoardTaskResponseDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBoardTask([FromBody] UpdateBoardTaskRequestDto boardTaskRequest)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var updatedBoardTaskResult = await _boardTaskService.UpdateAsync(userId, boardTaskRequest);

        return updatedBoardTaskResult.IsSuccess
            ? Ok(updatedBoardTaskResult.Value)
            : Problem(
                title: updatedBoardTaskResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: updatedBoardTaskResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpDelete("{taskId:int}")]
    [ProducesResponseType(typeof(string),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBoardTask([FromRoute] int taskId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var deletedBoardTaskResult = await _boardTaskService.DeleteAsync(userId, taskId);

        return deletedBoardTaskResult.IsSuccess
            ? Ok(deletedBoardTaskResult.Value)
            : Problem(
                title: deletedBoardTaskResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: deletedBoardTaskResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpPut("reorder")]
    [ProducesResponseType(typeof(BoardTaskPositionDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderBoardTask([FromBody] ReorderBoardTaskRequestDto boardTaskRequest)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var reorderedBoardTaskResult = await _boardTaskService.ReorderBoardTasksAsync(userId, boardTaskRequest);

        return reorderedBoardTaskResult.IsSuccess
            ? Ok(reorderedBoardTaskResult.Value)
            : Problem(
                title: reorderedBoardTaskResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: reorderedBoardTaskResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

}