using FluentResults;
using KanbanAppApi.Dtos;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services;

public interface IBoardTaskService
{
    Task<Result<BoarTaskDto>> CreateAsync(Guid userId, CreateBoardTaskRequestDto boardTaskRequest);
    Task<Result<UpdateBoardTaskResponseDto>> UpdateAsync(Guid userId, UpdateBoardTaskRequestDto boardTaskRequest);
    Task<Result<string>> DeleteAsync(Guid userId, int boardTaskId);
    Task<Result<List<BoardTaskPositionDto>>> ReorderBoardTasksAsync(Guid userId, ReorderBoardTaskRequestDto boardTaskRequest);
}