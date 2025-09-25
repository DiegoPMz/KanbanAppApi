using KanbanAppApi.Dtos;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public interface IBoardService
    {
        Task<ApiResponse<BoardDto?>> GetBoardAsync(Guid userId, int boardId);
        Task<ApiResponse<BoardDto?>> CreateBoardAsync(Guid userId, CreateBoardRequest requestBoard);
        Task<ApiResponse<UpdateBoardNameDto?>> UpdateBoardNameAsync(Guid userId, UpdateBoardRequest requestBoard);
        Task<ApiResponse<object?>> DeleteBoardAsync(Guid userId, int boardId);
    }
}
