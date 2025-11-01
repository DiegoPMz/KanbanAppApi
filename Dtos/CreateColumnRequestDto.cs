using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos;

public record CreateColumnRequestDto(
    [Range(1, int.MaxValue, ErrorMessage = "Invalid board Id. Must be greater than 0.")]
    int BoardId,

    [Required(ErrorMessage = "Column name is required.")]
    [MinLength(1, ErrorMessage = "Column name must not be empty.")]
    string Name,

    [Required(ErrorMessage = "Column color is required.")]
    [MinLength(1, ErrorMessage = "Column color must not be empty.")]
    string Color
);