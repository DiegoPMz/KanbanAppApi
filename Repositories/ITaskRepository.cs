namespace KanbanAppApi.Repositories
{
    public interface ITaskRepository
    {
        Task<Models.Task> CreateTaskAsync(int columnId, Models.Task task);
        Task<IEnumerable<Models.Task>> GetTasksByColumnIdAsync(int columnId);
        Task<Models.Task?> GetTaskByIdAsync(int columnId, int taskId);
        Task<Models.Task?> UpdateTaskAsync(int columnId, Models.Task task);
        void DeleteTask(int columnId, int taskId);
    }
}
