using System.ComponentModel.DataAnnotations;

namespace KanbanAppApi.Dtos
{
    public record ReorderColumnRequestDto(
        int BoardId,
        int Id,
        [Range(1,int.MaxValue)]int Position
    );

}
