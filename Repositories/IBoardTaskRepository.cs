using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface IBoardTaskRepository
    {
        Task<BoardTask?> CreateBoardTaskAsync(BoardTask boardTask);
        Task<IEnumerable<BoardTask>> GetBoardTasksByColumnIdAsync(int columnId);
        Task<BoardTask?> GetBoardTaskByIdAsync(int boardTaskId);
        Task<BoardTask?> UpdateBoardTaskAsync(BoardTask boardTask);
        Task DeleteTask(BoardTask boardTask);
    }
}
