using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetBySubAsync(string sub);
}