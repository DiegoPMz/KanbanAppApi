using KanbanAppApi.Dtos;
using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface IBoardRepository
    {
        Task<Board?> CreateBoardAsync(Board board);
        Task<Board?> GetBoardByIdAsync(int boardId);
        Task<IEnumerable<BoardSummaryDto>> GetBoardSummariesByUserIdAsync(Guid userId);
        Task<Board?> UpdateBoardAsync(Board board);
        Task DeleteBoardAsync(Board board);
    }
}
