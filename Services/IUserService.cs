using FluentResults;
using KanbanAppApi.Models;

namespace KanbanAppApi.Services;

public interface IUserService
{
    Task<Result<User>> CreateFromSubAsync(string sub,string email);
    Task<Result<User>> GetBySubAsync(string sub);
    Task<Result<User>> GetByIdAsync(Guid userId);
    Task<Result<string>> UpdateTheme(Guid userId, string theme);
}