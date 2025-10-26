using System.Net;
using FluentResults;

namespace KanbanAppApi.Errors;

public static class UserErrors
{
    public static Error NotFound(Guid id) => new Error($"User not found with id: {id.ToString()}")
        .WithMetadata(ErrorsMetadata.ErrorCode, "USER_NOT_FOUND")
        .WithMetadata(ErrorsMetadata.Type, "Business")
        .WithMetadata(ErrorsMetadata.Entity, "User")
        .WithMetadata(ErrorsMetadata.Id, id.ToString())
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.NotFound);

    public static Error NotFoundBySub(string sub) => new Error($"User not found with sub: {sub}")
        .WithMetadata(ErrorsMetadata.ErrorCode, "USER_NOT_FOUND")
        .WithMetadata(ErrorsMetadata.Type, "Business")
        .WithMetadata(ErrorsMetadata.Entity, "User")
        .WithMetadata(ErrorsMetadata.Sub, sub)
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.NotFound);
}