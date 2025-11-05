using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KanbanAppApi.Models.Enums;

namespace KanbanAppApi.Dtos;

public record CreateBoardTaskRequestDto (
    [Range(1, int.MaxValue, ErrorMessage = "Invalid Column Id.")]
    int ColumnId,

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title cannot be empty.")]
    string Title,

    [MinLength(1, ErrorMessage = "Description cannot be empty.")]
    string? Description,

    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    [Required(ErrorMessage = "Priority is required.")]
    PriorityType Priority
);


