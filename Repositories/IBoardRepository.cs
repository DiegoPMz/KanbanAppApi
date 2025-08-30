using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface IBoardRepository
    {
        Task<Board> CreateBoardAsync(int userId, Board board);

        Task<Board?> GetBoardByIdAsync(int userId, int boardId);

        Task<IEnumerable<Board>> GetAllBoardsAsync(int userId);

        Task<Board> UpdateBoardAsync(int userId, Board board);

        System.Threading.Tasks.Task DeleteBoardAsync(int userId, int boardId);
    }
}
