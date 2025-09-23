using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public interface IUserService
    {
        Task<User?> CreateUserFromSubAsync(string sub,string email);
        Task<User?> GetUserBySubAsync(string sub);
        Task<ApiResponse<UserProfileDto?>> GetUserBoardSummariesByIdAsync(Guid userId);
        Task<ApiResponse<object?>> UpdateAppTheme(Guid userId, string theme);
    }
}
