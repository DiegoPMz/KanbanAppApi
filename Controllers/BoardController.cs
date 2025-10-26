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
[Route("api/boards")]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;
    
    public BoardController(IBoardService boardService) => _boardService = boardService;

    [HttpGet("{boardId:int}")]
    [ProducesResponseType(typeof(BoardDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBoard(int boardId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var boardResult = await _boardService.GetByIdAsync(userId, boardId);
        return boardResult.IsSuccess
            ? Ok(boardResult.Value)
            : Problem(
                title: boardResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: boardResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(BoardDto),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest requestBoard )
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var createBoardResult = await _boardService.CreateAsync(userId, requestBoard);

        return createBoardResult.IsSuccess
            ? CreatedAtAction(
                nameof(GetBoard), 
                new { boardId  = createBoardResult.Value.Id }, 
                createBoardResult.Value
            )
            : Problem(
                title: createBoardResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: createBoardResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpPut]
    [ProducesResponseType(typeof(UpdateBoardResponseDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBoard([FromBody] UpdateBoardRequest requestBoard)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var updateBoardResult = await _boardService.UpdateAsync(userId, requestBoard);

        return updateBoardResult.IsSuccess
            ? Ok(updateBoardResult.Value)
            : Problem(
                title: updateBoardResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: updateBoardResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }

    [HttpDelete("{boardId:int}")]
    [ProducesResponseType(typeof(string),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBoard(int boardId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var deleteBoardResult = await _boardService.DeleteAsync(userId, boardId);

        return deleteBoardResult.IsSuccess 
            ? Ok(deleteBoardResult.Value) 
            : Problem(
                title: deleteBoardResult.Errors[0].Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: deleteBoardResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }
}