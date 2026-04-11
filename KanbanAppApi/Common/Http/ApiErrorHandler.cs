using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KanbanAppApi.Common.Http;

public static class ApiErrorHandler
{
    public static ProblemHttpResult Problem(ErrorOr.Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return TypedResults.Problem(
            statusCode: statusCode,
            title: error.Code, 
            detail: error.Description,
            extensions: new Dictionary<string, object?> 
            { 
                { "error_type", error.Type.ToString() } 
            }
        );
    }
}