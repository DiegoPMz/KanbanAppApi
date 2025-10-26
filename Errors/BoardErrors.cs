using System.Net;
using FluentResults;

namespace KanbanAppApi.Errors;

public static class BoardErrors
{
    public static Error NotFound(string id, string? identifier = "Id") => new Error($"Board not found with {identifier}: {id}")
        .WithMetadata(ErrorsMetadata.ErrorCode, "BOARD_NOT_FOUND")
        .WithMetadata(ErrorsMetadata.Type, "Business")
        .WithMetadata(ErrorsMetadata.Entity, "Board")
        .WithMetadata(ErrorsMetadata.Identifier, identifier)
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.NotFound);
}