using KanbanAppApi.Dtos;
using KanbanAppApi.Filters;
using KanbanAppApi.Responses;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KanbanAppApi.Controllers
{
    [Authorize]
    [RequireUserId]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<
            Results<
                Ok<ApiResponse<UserProfileDto?>>,
                BadRequest<ApiResponse<UserProfileDto?>>
                >
            > 
            GetUser()
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var userProfile = await _userService.GetUserBoardSummariesByIdAsync(userId);

            return userProfile.Succeeded 
                ? TypedResults.Ok(userProfile)
                : TypedResults.BadRequest(userProfile);
        }

        [HttpPut("theme")]
        public async Task<
            Results<
                Ok<ApiResponse<object?>>,
                BadRequest<ApiResponse<object?>>
                >
            > 
            SetTheme([FromBody] ChangeThemeRequest request)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var serviceResponse = await _userService.UpdateAppTheme(userId, request.Theme);

            return serviceResponse.Succeeded 
                ? TypedResults.Ok(serviceResponse)
                : TypedResults.BadRequest(serviceResponse);
        }
    }
}
