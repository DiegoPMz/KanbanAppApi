using KanbanAppApi.Dtos;
using KanbanAppApi.Errors;
using KanbanAppApi.Filters;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanbanAppApi.Controllers;

[Authorize]
[RequireUserId]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) => _userService = userService;

    [HttpGet]
    [ProducesResponseType(typeof(UserDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserDetails()
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var getUserResult = await _userService.GetByIdAsync(userId);
        
        return getUserResult.IsSuccess
            ? Ok(new UserDto(
                getUserResult.Value.Id, 
                getUserResult.Value.Email, 
                getUserResult.Value.AppTheme
                )
            )
            : Problem(
                detail: getUserResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound,
                title: getUserResult.Errors[0]?.Metadata[ErrorsMetadata.ErrorCode]?.ToString()
            );
    }

    [HttpPut("theme")]
    [ProducesResponseType(typeof(string),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTheme([FromQuery] string theme)
    {
        if (string.IsNullOrEmpty(theme) || theme.Length < 4)
        {
            return Problem(
                title: "Invalid theme",
                detail: "Theme must be at least 4 characters",
                statusCode: StatusCodes.Status400BadRequest
            );
        }
        
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var updateThemeResult = await _userService.UpdateTheme(userId, theme);
        
        return updateThemeResult.IsSuccess
            ? Ok(updateThemeResult.Value)
            : Problem(
                title: updateThemeResult.Errors[0]?.Metadata[ErrorsMetadata.ErrorCode]?.ToString(),
                detail: updateThemeResult.Errors[0].Message,
                statusCode: StatusCodes.Status404NotFound
            );
    }
}