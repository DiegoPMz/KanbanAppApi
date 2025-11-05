using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories;

public interface IBoardTaskRepository
{
    Task<BoardTask> CreateAsync(BoardTask boardTask);
    Task<List<BoardTask>> GetByColumnIdAsync(int columnId);
    Task<BoardTask?> GetByIdAsync(int boardTaskId);
    Task<int> GetCountByColumnIdAsync(int columnId);
    Task<BoardTask> UpdateAsync(BoardTask boardTask);
    Task UpdatePositionsAsync(List<BoardTask> boardTasks);
    Task DeleteAsync(BoardTask boardTask);
}