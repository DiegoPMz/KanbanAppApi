using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories;

public interface IColumnRepository
{
    Task<Column> CreateAsync(Column column);
    Task<List<Column>> GetAllByBoardIdAsync(int boardId);
    Task<Column?> GetByIdAsync(int columnId);
    Task<List<Column>> GetAllWithBoardTasksAndSubtasksAsync(int columnId);
    Task<Column> UpdateAsync(Column column);
    Task DeleteAsync(Column column);
    Task UpdatePositionsAsync(List<Column> columns);
    Task<bool> ExistsForUserAsync(Guid userId ,int columnId);
}