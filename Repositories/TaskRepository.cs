
using KanbanAppApi.Data;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationContextDB _context;

        public TaskRepository(ApplicationContextDB context )
        {
            _context = context;
        }

        public async Task<Models.Task> CreateTaskAsync(int columnId, Models.Task task)
        {
            task.ColumnId = columnId;
            var newTask = await _context.Tasks.AddAsync(task);
            return newTask.Entity;
        }

        public void DeleteTask(int columnId, int taskId)
        {
            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == taskId && t.ColumnId == columnId);
            if (task == null) return;

            _context.Tasks.Remove(task);
        }

        public async Task<Models.Task?> GetTaskByIdAsync(int columnId, int taskId)
        {
           var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.ColumnId == columnId && t.Id == taskId);
            return task;
        }

        public async Task<IEnumerable<Models.Task>> GetTasksByColumnIdAsync(int columnId)
        {
            return await _context.Tasks
                .Where(t => t.ColumnId == columnId)
                .ToListAsync();
        }


        public async Task<Models.Task?> UpdateTaskAsync(int columnId, Models.Task task)
        {
            var existingTask = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == task.Id && t.ColumnId == columnId);

            if (existingTask == null) return null;

            _context.Entry(existingTask).CurrentValues.SetValues(task);
            await _context.SaveChangesAsync();
            return existingTask;
        }
    }
}
