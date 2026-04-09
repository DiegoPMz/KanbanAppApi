namespace KanbanAppApi.Features.Auth.Shared;

public record struct TokenDto(Guid SessionId, DateTime ExpiresAt);
