using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record ReorderBoardTaskRequestDto(
        int Id,
        int ColumnId,
        [Range(1, int.MaxValue)] int Position
    );
}
