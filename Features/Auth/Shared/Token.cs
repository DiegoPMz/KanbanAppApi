using ErrorOr;

namespace KanbanAppApi.Features.Auth.Shared;

public sealed record TokenCreatedEvent(string Sub, string Provider);

public enum TokenType {
    AccessToken,
    RefreshToken,
}

public class Token
{
    public Guid SessionId { get; private init; }
    public string Provider { get; private set; }
    public TokenType Type { get; private init; }
    public DateTime ExpiresAt { get; private init; }
    public Guid UserId { get; private init; }
    
    private Token() { }
    private Token(Guid userId, string provider, TokenType type,  DateTime expiresAt)
    {
        UserId = userId;
        Provider = provider;
        Type = type;
        ExpiresAt = expiresAt;
    }

    public static ErrorOr<Token> Create(
        Guid userId, 
        string provider, 
        TokenType type, 
        DateTime expiresAt)
    {
        List<Error> errors = [];

        if (userId == Guid.Empty)
            errors.Add(TokenErrors.InvalidUserId);

        if (string.IsNullOrWhiteSpace(provider))
            errors.Add(TokenErrors.ProviderRequired);

        if (!Enum.IsDefined(typeof(TokenType), type))
            errors.Add(TokenErrors.InvalidType);

        if (expiresAt <= DateTime.UtcNow)
            errors.Add(TokenErrors.InvalidExpiration);

        if (errors.Count > 0) return errors;

        return new Token(userId, provider.Trim(), type, expiresAt);
    }
}