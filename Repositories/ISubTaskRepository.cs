using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories;

public interface ISubTaskRepository
{
    Task<SubTask> CreateAsync(SubTask subTask);
    Task<IEnumerable<SubTask>> GetAllByBoardTaskIdAsync(int boardTaskId);
    Task<SubTask?> GetByIdAsync(int subTaskId);
    Task<SubTask> UpdateAsync(SubTask subTask);
    Task DeleteAsync(SubTask subTask);
    Task<bool> UserOwnsSubTaskAsync(Guid userId, int subTaskId);
}