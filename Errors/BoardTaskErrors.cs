using System.Net;
using FluentResults;

namespace KanbanAppApi.Errors;

public static class BoardTaskErrors
{
    public static Error NotFound(string id, string? identifier = "Id") => new Error($"Board task not found with {identifier}: {id}")
        .WithMetadata(ErrorsMetadata.ErrorCode, "BOARD_TASK_NOT_FOUND")
        .WithMetadata(ErrorsMetadata.Type, "Business")
        .WithMetadata(ErrorsMetadata.Entity, "BoardTask")
        .WithMetadata(ErrorsMetadata.Identifier, identifier)
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.NotFound);
    
    public static Error InvalidPosition(int maxPosition) => new Error($"Invalid position. Allowed range is from 1 to {maxPosition}")
        .WithMetadata(ErrorsMetadata.ErrorCode, "INVALID_BOARD_TASK_POSITION")
        .WithMetadata(ErrorsMetadata.Type, "Validation")
        .WithMetadata(ErrorsMetadata.Entity, "BoardTask")
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.BadRequest);
}