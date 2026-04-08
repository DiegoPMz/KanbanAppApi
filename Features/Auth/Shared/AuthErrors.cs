using ErrorOr;

namespace KanbanAppApi.Features.Auth.Shared;

public class AuthErrors
{
    public static Error ExternalProviderError(string provider, string details) => Error.Failure(
        code: "AUTH.EXTERNAL_PROVIDER_ERROR",
        description: $"Error communicating with {provider}. Details: {details}");

    public static Error InvalidExternalCode = Error.Validation(
        code: "AUTH.INVALID_EXTERNAL_CODE",
        description: "The authorization code provided by the external issuer is invalid or has expired.");

    public static Error TokenExchangeFailed = Error.Failure(
        code: "AUTH.TOKEN_EXCHANGE_FAILED",
        description: "Failed to exchange authorization code for access tokens.");


    public static Error InvalidIdToken = Error.Unauthorized(
        code: "AUTH.INVALID_ID_TOKEN",
        description: "The identity token (id_token) signature or claims are invalid.");

    public static Error MissingRequiredClaims = Error.Validation(
        code: "AUTH.MISSING_CLAIMS",
        description: "The provider did not return the required user information (email or subject).");

    public static Error EmailNotVerified = Error.Unauthorized(
        code: "AUTH.EMAIL_NOT_VERIFIED",
        description: "Your external account email must be verified to log in.");


    public static Error SessionNotFound = Error.NotFound(
        code: "AUTH.SESSION_NOT_FOUND",
        description: "The provided session identifier is invalid or has expired.");
    
    
    public static Error UserLockedOut = Error.Forbidden(
        code: "AUTH.USER_LOCKED",
        description: "This user account has been deactivated or locked.");
    
    public static Error ProviderKeysUnavailable = Error.Failure(
        code: "AUTH.PROVIDER_KEYS_UNAVAILABLE",
        description: "Could not retrieve the public signing keys from the identity provider.");
    
    public static Error IdentityVerificationFailed = Error.Unauthorized(
        code: "AUTH.IDENTITY_VERIFICATION_FAILED",
        description: "The provided identity token could not be verified against the provider's public keys.");
}