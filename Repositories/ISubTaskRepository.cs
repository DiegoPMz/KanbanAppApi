namespace KanbanAppApi.Repositories
{
    public interface ISubTaskRepository
    {
        Task<Models.SubTask> CreateSubTaskAsync(int taskId, Models.SubTask subTask);
        Task<IEnumerable<Models.SubTask>> GetSubTasksByTaskIdAsync(int taskId);
        Task<Models.SubTask?> GetSubTaskByIdAsync(int taskId, int subTaskId);
        Task<Models.SubTask?> UpdateSubTaskAsync(int taskId, Models.SubTask subTask);
        void DeleteSubTask(int taskId, int subTaskId);
    }
}
