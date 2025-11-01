using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos;

public record UpdateColumnRequestDto(
    [Range(1, int.MaxValue, ErrorMessage = "Invalid board Id")]
    int BoardId,

    [Range(1, int.MaxValue, ErrorMessage = "Invalid column Id")]
    int Id,

    [MinLength(1, ErrorMessage = "Column name must not be empty.")]
    string? Name,

    [MinLength(1, ErrorMessage = "Column color must not be empty.")]
    string? Color
);