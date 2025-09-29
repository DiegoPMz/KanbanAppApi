using KanbanAppApi.Dtos;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public interface IBoardTaskService
    {
        Task<ApiResponse<BoardTaskResponseDto?>> CreateBoardTaskAsync(Guid userId, CreateBoardTaskRequestDto boardTaskRequest);
        Task<ApiResponse<BoardTaskResponseDto?>> UpdateBoardTaskAsync(Guid userId, UpdateBoardTaskRequestDto boardTaskRequest);
        Task<ApiResponse<object?>> DeleteBoardTaskAsync(Guid userId, int boardTaskId);
        Task<ApiResponse<List<BoardTaskPositionDto>?>> ReorderBoardTaskAsync(Guid userId, ReorderBoardTaskRequestDto boardTaskRequest);
    }
}
