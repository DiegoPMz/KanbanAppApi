using System.Text.Json;
using System.Text.Json.Serialization;
using ErrorOr;
using KanbanAppApi.Features.Auth.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace KanbanAppApi.Features.Auth.Infrastructure;

public record GoogleTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("refresh_token")] string? RefreshToken,
    [property: JsonPropertyName("scope")] string Scope,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("id_token")] string IdToken
);

public sealed class GoogleAuthProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory) 
    : Login.IExternalAuthProvider
{
    private const string GoogleAuthEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string GoogleTokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string GoogleCertsEndpoint = "https://www.googleapis.com/oauth2/v3/certs";
    
    public ErrorOr<string> GetProviderPkceUrl(string codeChallenge)
    {
        var googleQuery = new Dictionary<string, string?>
        {
            ["client_id"] = configuration["Google:ClientId"],
            ["response_type"] = "code",
            ["scope"] = "openid email profile",
            ["redirect_uri"] = configuration["Google:CallbackRedirect"],
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = "S256",
            ["prompt"] = "consent"
        };

        return QueryHelpers.AddQueryString(
            GoogleAuthEndpoint,
            googleQuery
        );
    }

    public async Task<ErrorOr<Login.ExternalCodeExchangeResponse>> ExchangeCodeForTokenAsync(
        string code, 
        string codeVerifier, 
        CancellationToken ct
        )
    {
        var googleParameters = new Dictionary<string, string?>
        {
            ["client_id"] = configuration["Google:ClientId"],
            ["client_secret"] = configuration["Google:ClientSecret"],
            ["code_verifier"] = codeVerifier,
            ["code"] = code,
            ["redirect_uri"] = configuration["Google:CallbackRedirect"],
            ["grant_type"] = "authorization_code",
        };
     
        var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.PostAsync(
            GoogleTokenEndpoint,
            new FormUrlEncodedContent(googleParameters),
            ct
        );

        Console.WriteLine(response.Content.ReadAsStringAsync(ct));
        
        if (!response.IsSuccessStatusCode)
        {
            // var error = await response.Content.ReadAsStringAsync(ct);
            return AuthErrors.TokenExchangeFailed;
        }

        GoogleTokenResponse? responseBody = null;

        try 
        {
            responseBody = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>(ct);
        }
        catch (Exception ex) 
        {
            return ExternalCommunicationExceptionHandler<Login.ExternalCodeExchangeResponse>(ex, "Google");
        }
        
        if (responseBody is null) 
            return AuthErrors.ExternalProviderError("Google", "The response body was empty or null.");
            
        return new Login.ExternalCodeExchangeResponse(
            responseBody.AccessToken,
            responseBody.RefreshToken,
            responseBody.Scope,
            responseBody.TokenType,
            responseBody.IdToken
        );
    }

    public async Task<ErrorOr<Login.ExternalUserIdentity>> GetUserDetailsAsync(string tokenId, CancellationToken ct)
    {
        JsonWebKeySet? jwks = null;

        try 
        {
            jwks = await httpClientFactory
                .CreateClient()
                .GetFromJsonAsync<JsonWebKeySet>(GoogleCertsEndpoint, ct);
        }
        catch (Exception ex) 
        {
            return ExternalCommunicationExceptionHandler<Login.ExternalUserIdentity>(ex, "Google");
        }

        if (jwks is null || jwks.Keys.Count == 0) 
            return AuthErrors.ExternalProviderError("Google", "Received an empty key set.");
        
        var tokenHandler = new JsonWebTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = ["accounts.google.com", "https://accounts.google.com"],
            ValidateAudience = true,
            ValidAudience = configuration["Google:ClientId"],
            ValidateLifetime = true,
            IssuerSigningKeys = jwks.Keys
        };

        var validation = await tokenHandler.ValidateTokenAsync(tokenId, validationParameters);
        if (!validation.IsValid)  
            return AuthErrors.IdentityVerificationFailed;

        var claims = validation.Claims;
        var sub = Get("sub");
        var email = Get("email");
        
        if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(email))
            return AuthErrors.MissingRequiredClaims;

        return new Login.ExternalUserIdentity 
        {
            Provider = "Google" ,
            ProviderKey = sub, 
            Email = email,
            UrlPicture = Get("picture"),
            Name = Get("name"),
            GivenName = Get("given_name"),
            FamilyName = Get("family_name"),
            IsEmailVerified = claims.TryGetValue("email_verified", out var ev) && 
                              (ev is bool b ? b : ev.ToString()?.ToLower() == "true")
        };

        string? Get(string key) => claims.TryGetValue(key, out var val) ? val?.ToString() : null;
    }
    
    private static ErrorOr<TR> ExternalCommunicationExceptionHandler<TR>(Exception ex, string providerName)
    {
        var detail = ex switch
        {
            HttpRequestException => $"Could not fetch {providerName} certificates or connect to the service.",
            JsonException => $"{providerName} response format is invalid.",
            OperationCanceledException => "The request timed out.",
            _ => $"An unexpected error occurred while communicating with {providerName}."
        };

        return AuthErrors.ExternalProviderError(providerName, detail);
    }
}