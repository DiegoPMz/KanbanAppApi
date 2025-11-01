using System.Net;
using FluentResults;

namespace KanbanAppApi.Errors;

public static class ColumnErrors
{
    public static Error NotFound(string id, string? identifier = "Id") => new Error($"Column not found with {identifier}: {id}")
        .WithMetadata(ErrorsMetadata.ErrorCode, "COLUMN_NOT_FOUND")
        .WithMetadata(ErrorsMetadata.Type, "Business")
        .WithMetadata(ErrorsMetadata.Entity, "Column")
        .WithMetadata(ErrorsMetadata.Identifier, identifier)
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.NotFound);
    
    public static Error InvalidPosition(int maxPosition) => new Error($"Invalid position. Allowed range is from 1 to {maxPosition}")
        .WithMetadata(ErrorsMetadata.ErrorCode, "INVALID_COLUMN_POSITION")
        .WithMetadata(ErrorsMetadata.Type, "Validation")
        .WithMetadata(ErrorsMetadata.Entity, "Column")
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.BadRequest);
}