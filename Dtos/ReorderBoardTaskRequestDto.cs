using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos;

public record ReorderBoardTaskRequestDto(
    [Range(1, int.MaxValue, ErrorMessage = "Invalid task Id.")]
    int Id,

    [Range(1, int.MaxValue, ErrorMessage = "Invalid Column Id.")]
    int ColumnId,

    [Range(1, int.MaxValue, ErrorMessage = "Invalid position. It must be 1 or greater.")]
    int Position
);