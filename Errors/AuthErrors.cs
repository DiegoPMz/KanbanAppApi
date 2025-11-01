using System.Net;
using FluentResults;

namespace KanbanAppApi.Errors;

public static class AuthErrors
{
    public static Error MissingRefreshToken() => new Error("Missing refresh token.")
        .WithMetadata(ErrorsMetadata.ErrorCode, "MISSING_REFRESH_TOKEN")
        .WithMetadata(ErrorsMetadata.Type, "Authentication")
        .WithMetadata(ErrorsMetadata.Entity, "Auth")
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.Unauthorized);
    
    public static Error InvalidRefreshToken() => new Error("Invalid refresh token.")
        .WithMetadata(ErrorsMetadata.ErrorCode, "INVALID_REFRESH_TOKEN")
        .WithMetadata(ErrorsMetadata.Type, "Authentication")
        .WithMetadata(ErrorsMetadata.Entity, "Auth")
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.Unauthorized);
}