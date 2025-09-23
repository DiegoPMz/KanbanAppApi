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

        public async Task<User?> CreateUserAsync(User user)
        {
            var userDb = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return userDb.Entity;
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserBySubAsync(string sub)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Sub == sub);
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
