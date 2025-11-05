using System.Net;
using FluentResults;

namespace KanbanAppApi.Errors;

public static class SubtaskErrors
{
    public static Error NotFound(string id, string? identifier = "Id") => new Error($"Subtask not found with {identifier}: {id}")
        .WithMetadata(ErrorsMetadata.ErrorCode, "SUBTASK_NOT_FOUND")
        .WithMetadata(ErrorsMetadata.Type, "Business")
        .WithMetadata(ErrorsMetadata.Entity, "Subtask")
        .WithMetadata(ErrorsMetadata.Identifier, identifier)
        .WithMetadata(ErrorsMetadata.HttpStatus, HttpStatusCode.NotFound);
    
}