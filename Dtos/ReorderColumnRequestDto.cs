using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos;

public record ReorderColumnRequestDto(
    [Range(1, int.MaxValue, ErrorMessage = "Invalid board Id")]
    int BoardId,

    [Range(1, int.MaxValue, ErrorMessage = "Invalid column Id")]
    int Id,

    [Range(1, int.MaxValue, ErrorMessage = "Invalid position")]
    int Position
);