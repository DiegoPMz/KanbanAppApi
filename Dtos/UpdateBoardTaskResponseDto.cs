using KanbanAppApi.Models.Enums;

namespace KanbanAppApi.Dtos;

public record UpdateBoardTaskResponseDto(
    int Id,
    string Title,
    string Description, 
    int ColumnId, 
    bool IsCompleted,
    PriorityType Priority
);