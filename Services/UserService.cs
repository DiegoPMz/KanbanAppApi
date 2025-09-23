using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;
using KanbanAppApi.Responses;

namespace KanbanAppApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBoardRepository _boardRepository;

        public UserService(IUserRepository userRepository, IBoardRepository boardRepository )
        {
            _userRepository= userRepository;
            _boardRepository = boardRepository;
        }

        public async Task<User?> CreateUserFromSubAsync(string sub, string email)
        {
            User newUser = new()
            {
                Email = email,
                Sub = sub,
                AppTheme = "Light"
            };

            return await _userRepository.CreateUserAsync(newUser);
        }

        public async Task<ApiResponse<UserProfileDto?>> GetUserBoardSummariesByIdAsync(Guid userId)
        {
            User? userDb = await _userRepository.GetUserByIdAsync(userId);
            if (userDb is null) return ApiResponse<UserProfileDto?>.Failure("The user Id is invalid", []);

            IEnumerable<BoardSummaryDto> userBoards = await _boardRepository.GetBoardSummariesByUserIdAsync(userId) ?? [];
            var responseData = new UserProfileDto(
                userDb.Id,
                userDb.Email,
                userDb.AppTheme,
                userBoards.ToList()
            );

            return ApiResponse<UserProfileDto?>.Success(responseData,"Operation successfuly");
        }

        public async Task<User?> GetUserBySubAsync(string sub)
        {
            return await _userRepository.GetUserBySubAsync(sub);
        }

        public async Task<ApiResponse<object?>> UpdateAppTheme(Guid userId, string theme)
        {
            var userDb = await _userRepository.GetUserByIdAsync(userId);
            if (userDb is null) return ApiResponse<object?>.Failure("The user Id is invalid", []);

            userDb.AppTheme = theme;
            await _userRepository.UpdateUserAsync(userDb);

            return ApiResponse<object?>.Success(null,"App Theme changed");
        }
    }
}
