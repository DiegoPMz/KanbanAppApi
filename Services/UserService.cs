using FluentResults;
using KanbanAppApi.Errors;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;

namespace KanbanAppApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository) =>  _userRepository = userRepository;
    
    public async Task<Result<User>> CreateFromSubAsync(string sub, string email)
    {
        var user = await _userRepository.CreateAsync(new User(sub, email));
        return user;
    }
        
    public async Task<Result<User>> GetBySubAsync(string sub)
    {
        var user = await _userRepository.GetBySubAsync(sub);
        return user is null
            ? UserErrors.NotFoundBySub(sub)
            : user;                         
    }
    
    public async Task<Result<User>> GetByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user is null
            ? UserErrors.NotFound(userId)
            : user;    
    }

    public async Task<Result<string>> UpdateTheme(Guid userId, string theme)
    {
        var userDb = await _userRepository.GetByIdAsync(userId);
        if (userDb is null) return UserErrors.NotFound(userId);

        userDb.AppTheme = theme;
        await _userRepository.UpdateAsync(userDb);

        return "Application theme updated successfully";
    }
}