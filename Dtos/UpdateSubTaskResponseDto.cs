namespace KanbanAppApi.Dtos;

public record UpdateSubTaskResponseDto(
    string Description, 
    bool IsCompleted
);