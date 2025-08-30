using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface IColumnRepository
    {
        Task<Column> CreateColumnAsync(int boardId, Column column);
        Task<IEnumerable<Column>> GetColumnsByBoardIdAsync(int boardId);
        Task<Column?> GetColumnByIdAsync(int boardId, int columnId);
        Task<Column?> UpdateColumnAsync(int boardId, Column column);
        System.Threading.Tasks.Task DeleteColumnAsync(int boardId, int columnId);
    }
}
