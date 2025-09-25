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
    [Route("api/board")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet("{boardId}")]
        public async Task<IResult> GetBoard([FromRoute] int boardId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var response = await _boardService.GetBoardAsync(userId, boardId);

            return response.Succeeded
                 ? TypedResults.Ok(response)
                 : TypedResults.BadRequest(response);
        }

        [HttpPost]
        public async Task<IResult> CreateBoard([FromBody] CreateBoardRequest requestBoard )
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var response = await _boardService.CreateBoardAsync(userId, requestBoard);

             return response.Succeeded
                 ? TypedResults.Ok(response)
                 : TypedResults.BadRequest(response);
        }

        [HttpPut]
        public async Task<IResult> UpdateBoard([FromBody] UpdateBoardRequest requestBoard)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var response = await _boardService.UpdateBoardNameAsync(userId, requestBoard);

            return response.Succeeded
                 ? TypedResults.Ok(response)
                 : TypedResults.BadRequest(response);
        }

        [HttpDelete("{boardId}")]
        public async Task<IResult> DeleteBoard([FromRoute] int boardId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var response = await _boardService.DeleteBoardAsync(userId, boardId);

            return response.Succeeded 
                ? TypedResults.Ok(response) 
                : TypedResults.BadRequest(response);
        }

    }
}
