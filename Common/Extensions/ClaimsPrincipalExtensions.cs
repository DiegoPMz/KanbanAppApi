using System.Security.Claims;

namespace KanbanAppApi.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    private const string UserIdClaimName = "userId";
    
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue(UserIdClaimName);
        return Guid.TryParse(claimValue, out var userId) ? userId : null;
    }
    
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        return user.GetUserId() 
               ?? throw new UnauthorizedAccessException("User ID claim is missing.");
    }
}