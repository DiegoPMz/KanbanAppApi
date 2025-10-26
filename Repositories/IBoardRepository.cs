using KanbanAppApi.Dtos;
using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories;

public interface IBoardRepository
{
    Task<Board> CreateAsync(Board board);
    Task<Board?> GetByIdAsync(int boardId);
    Task<IEnumerable<Board>> GetAllByUserId(Guid userId);
    Task<Board> UpdateAsync(Board board);
    Task DeleteAsync(Board board);
    Task<bool> ExistsForUserAsync(Guid userId, int boardId);

}