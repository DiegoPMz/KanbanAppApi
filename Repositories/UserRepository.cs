using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContextDb _context;

    public UserRepository(ApplicationContextDb context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User user)
    {
        var userDb = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return userDb.Entity;
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetBySubAsync(string sub)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Sub == sub);
    }

    public async Task<User?> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}