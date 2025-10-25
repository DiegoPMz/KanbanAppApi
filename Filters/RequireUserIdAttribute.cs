using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace KanbanAppApi.Filters
{
    public class RequireUserIdAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimId) || !Guid.TryParse(claimId, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Items["UserId"] = userId;
            await next();
        }


    }
}
