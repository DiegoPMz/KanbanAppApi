
using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories
{
    public class BoardTaskRepository : IBoardTaskRepository
    {
        private readonly ApplicationContextDB _context;

        public BoardTaskRepository(ApplicationContextDB context )
        {
            _context = context;
        }

        public async Task<BoardTask?> CreateBoardTaskAsync(BoardTask boardTask)
        {
            var newTask = await _context.BoardTask.AddAsync(boardTask);
            await _context.SaveChangesAsync();
            return newTask.Entity;
        }

        public async Task DeleteBoardTask(BoardTask boardTask)
        {
            _context.BoardTask.Remove(boardTask);
            await _context.SaveChangesAsync();           
        }

        public async Task<BoardTask?> GetBoardTaskByIdAsync(int boardTaskId)
        {
            return await _context.BoardTask.FirstOrDefaultAsync(bt => bt.Id == boardTaskId);
        }

        public async Task<IEnumerable<BoardTask>> GetBoardTasksByColumnIdAsync(int columnId)
        {
            return await _context.BoardTask
                .Where(bt => bt.ColumnId == columnId)
                .ToListAsync();
        }

        public async Task<BoardTask?> UpdateBoardTaskAsync(BoardTask boardTask)
        {
            _context.BoardTask.Update(boardTask);
            await _context.SaveChangesAsync();
            return boardTask;
        }

        public async Task UpdateBoardTasksPositionsAsync(List<BoardTask> boardTasks)
        {
            _context.BoardTask.UpdateRange(boardTasks);
            await _context.SaveChangesAsync();
        }
    }
}
