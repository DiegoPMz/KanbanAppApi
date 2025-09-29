using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record CreateBoardTaskRequestDto (
        [Required][MinLength(1)] string Title,
        [MinLength(1)] string? Description,
        int ColumnId
    );
}
