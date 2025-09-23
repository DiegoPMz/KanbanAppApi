using KanbanAppApi.Dtos;
using KanbanAppApi.Responses;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace KanbanAppApi.Controllers
{
    [Authorize]
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
                BadRequest<ApiResponse<UserProfileDto?>>,
                UnauthorizedHttpResult
                >
            > 
            GetUser()
        {
            string? claimId = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (claimId is null || !Guid.TryParse(claimId, out Guid userId ) ) return TypedResults.Unauthorized();

            ApiResponse<UserProfileDto?> userProfile = await _userService.GetUserBoardSummariesByIdAsync(userId);
            if (!userProfile.Succeeded) return TypedResults.BadRequest(userProfile);

            return TypedResults.Ok(userProfile);
        }

        [HttpPut("theme")]
        public async Task<
            Results<
                Ok<ApiResponse<object?>>,
                BadRequest<ApiResponse<object?>>,
                UnauthorizedHttpResult
                >
            > 
            SetTheme([FromBody] ChangeThemeRequest request)
        {
            string? claimId = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (claimId is null || !Guid.TryParse(claimId, out Guid userId) ) return TypedResults.Unauthorized();

            ApiResponse<object?> serviceResponse = await _userService.UpdateAppTheme(userId, request.Theme);
            if (serviceResponse is null) return TypedResults.BadRequest(serviceResponse);

            return TypedResults.Ok(serviceResponse);
        }
    }
}
