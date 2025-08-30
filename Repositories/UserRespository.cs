using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories
{
    public class UserRespository : IUserRepository
    {
        private readonly ApplicationContextDB _context;

        public UserRespository(ApplicationContextDB context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var entry = await _context.Users.AddAsync(user); 
            await _context.SaveChangesAsync();               
            return entry.Entity;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FindAsync(email);   
        }
        public async void AddUserAppTheme(string userId, string theme)
        {
            var currentUser = await _context.Users.FindAsync(userId);
            if (currentUser != null)
            {
                currentUser.AppTheme = theme;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UserAlreadyExistsAsync(string email)
        {
           return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
