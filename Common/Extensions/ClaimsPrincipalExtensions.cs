using System.Security.Claims;

namespace KanbanAppApi.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claimValue, out var userId) ? userId : null;
    }
    
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        return user.GetUserId() 
               ?? throw new UnauthorizedAccessException("User ID claim is missing.");
    }
}