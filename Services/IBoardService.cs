using FluentResults;
using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public interface IBoardService
    {
        Task<Result<BoardDto>> GetByIdAsync(Guid userId, int boardId);
        Task<Result<BoardDto>> CreateAsync(Guid userId, CreateBoardRequest requestBoard);
        Task<Result<UpdateBoardResponseDto>> UpdateAsync(Guid userId, UpdateBoardRequest requestBoard);
        Task<Result<string>> DeleteAsync(Guid userId, int boardId);
    }
}
