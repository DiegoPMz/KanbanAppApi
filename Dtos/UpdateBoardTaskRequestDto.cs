using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KanbanAppApi.Models.Enums;

namespace KanbanAppApi.Dtos;

public record UpdateBoardTaskRequestDto(
    [Range(1, int.MaxValue, ErrorMessage = "Invalid task Id.")]
    int Id,

    [MinLength(1, ErrorMessage = "Title cannot be empty.")]
    string? Title,

    [MinLength(1, ErrorMessage = "Description cannot be empty.")]
    string? Description,

    [Range(1, int.MaxValue, ErrorMessage = "Invalid Column Id.")]
    int? ColumnId,

    bool? IsCompleted,

    [EnumDataType(typeof(PriorityType), ErrorMessage = "Invalid priority value.")]
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    PriorityType? Priority
);