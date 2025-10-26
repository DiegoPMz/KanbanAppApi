using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos;

public record CreateBoardColumnRequest(
    [Required][MinLength(1)] string Name,
    [Required][MinLength(1)] string Color
);