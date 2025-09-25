using KanbanAppApi.Dtos;
using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface IColumnRepository
    {
        Task<Column?> CreateColumnAsync(Column column);
        Task<IEnumerable<Column>> GetColumnsByBoardIdAsync(int boardId);
        Task<Column?> GetColumnByIdAsync(int columnId);
        Task<List<ColumnDto>> GetColumnsWithBoardTasksAndSubtasksAsync(int columnId);
        Task<Column?> UpdateColumnAsync(Column column);
        Task DeleteColumnAsync(Column column);
    }
}
