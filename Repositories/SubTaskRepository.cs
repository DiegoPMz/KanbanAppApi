using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories
{
    public class SubTaskRepository : ISubTaskRepository
    {
        private readonly ApplicationContextDB _context;

        public SubTaskRepository(ApplicationContextDB context)
        {
            _context = context;
        }

        public async Task<SubTask?> CreateSubTaskAsync(SubTask subTask)
        {
            var newSubTask = await _context.Subtasks.AddAsync(subTask);
            return newSubTask.Entity;
        }

        public async Task DeleteSubTask(SubTask subTask)
        {
            _context.Subtasks.Remove(subTask);
            await _context.SaveChangesAsync();
        }

        public async Task<SubTask?> GetSubTaskByIdAsync(int subTaskId)
        {
            return await _context.Subtasks
                .FirstOrDefaultAsync(st => st.Id == subTaskId);
        }

        public async Task<IEnumerable<SubTask>> GetSubTasksByBoardTaskIdAsync(int boardTaskId)
        {
            return await _context.Subtasks
                .Where(st => st.BoardTaskId == boardTaskId)
                .ToListAsync();
        }

        public Task<bool> SubTaskExistsByUserIdAsync(Guid userId, int subTaskId)
        {
            return _context.Subtasks
                .AnyAsync(st =>
                    st.Id == subTaskId &&
                    st.BoardTask.Column.Board.UserId == userId
                );
        }

        public async Task<SubTask?> UpdateSubTaskAsync(SubTask subTask)
        {
            _context.Subtasks.Update(subTask);
            await _context.SaveChangesAsync();
            return subTask;
        }
    }
}
