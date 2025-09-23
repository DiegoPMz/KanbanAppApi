using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface ISubTaskRepository
    {
        Task<SubTask?> CreateSubTaskAsync(SubTask subTask);
        Task<IEnumerable<SubTask>> GetSubTasksByBoardTaskIdAsync(int boardTaskId);
        Task<SubTask?> GetSubTaskByIdAsync(int subTaskId);
        Task<SubTask?> UpdateSubTaskAsync(SubTask subTask);
        Task DeleteSubTask(SubTask subTask);
    }
}
