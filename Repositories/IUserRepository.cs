using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        void AddUserAppTheme(string userId ,string theme);
        Task<bool> UserAlreadyExistsAsync(string email);
    }
}
