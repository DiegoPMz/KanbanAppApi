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

        public async Task<SubTask> CreateSubTaskAsync(int taskId, SubTask subTask)
        {
            subTask.TaskId = taskId;
            var newSubTask = await _context.Subtasks.AddAsync(subTask);
            return newSubTask.Entity;
        }

        public void DeleteSubTask(int taskId, int subTaskId)
        {
            var subTask = _context.Subtasks
                .FirstOrDefault(st => st.Id == subTaskId && st.TaskId == taskId);
            if (subTask == null) return;
            
            _context.Subtasks.Remove(subTask);
        }


        public async Task<SubTask?> GetSubTaskByIdAsync(int taskId, int subTaskId)
        {
            return await _context.Subtasks
                .FirstOrDefaultAsync(st => st.Id == subTaskId && st.TaskId == taskId);
        }

        public async Task<IEnumerable<SubTask>> GetSubTasksByTaskIdAsync(int taskId)
        {
            return await _context.Subtasks
                .Where(st => st.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<SubTask?> UpdateSubTaskAsync(int taskId, SubTask subTask)
        {
            var existingSubTask = await _context.Subtasks
                .FirstOrDefaultAsync(st => st.Id == subTask.Id && st.TaskId == taskId);

            if (existingSubTask == null) return null;

            _context.Entry(existingSubTask).CurrentValues.SetValues(subTask);
            await _context.SaveChangesAsync();
            return existingSubTask;
        }
    }
}
