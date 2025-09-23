using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface IUserRepository
    {
        Task<User?> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserBySubAsync(string sub);
    }
}
