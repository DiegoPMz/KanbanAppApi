using ErrorOr;

namespace KanbanAppApi.Features.Auth.Shared;

public class TokenErrors
{
    public static Error InvalidUserId = Error.Validation(
        code: "TOKEN.INVALID_USER_ID",
        description: "The user identifier associated with the token is invalid.");

    public static Error ProviderRequired = Error.Validation(
        code: "TOKEN.PROVIDER_REQUIRED",
        description: "The authentication provider name is required.");

    public static Error InvalidExpiration = Error.Validation(
        code: "TOKEN.INVALID_EXPIRATION",
        description: "The token expiration date must be in the future.");
    
    public static Error InvalidType = Error.Validation(
        code: "TOKEN.INVALID_TYPE",
        description: "The provided token type is not supported.");
    
    public static Error TokenCreationFailed = Error.Failure(
        code: "AUTH.TOKEN_CREATION_FAILED",
        description: "An error occurred while generating the security token. Please try again.");
}