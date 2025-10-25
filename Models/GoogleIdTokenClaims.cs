namespace KanbanAppApi.Models;

public class GoogleIdTokenClaims
{
    public GoogleIdTokenClaims()
    {
    }

    public GoogleIdTokenClaims(string Sub, string Email, string Name, string GivenName, string FamilyName, string Picture)
    {
        this.Sub = Sub;
        this.Email = Email;
        this.Name = Name;
        this.GivenName = GivenName;
        this.FamilyName = FamilyName;
        this.Picture = Picture;
    }

    // Siempre presentes
    public string Aud { get; init; } = string.Empty; // client_id de tu app
    public long Exp { get; init; }                   // Unix epoch (expiración)
    public long Iat { get; init; }                   // Unix epoch (issued at)
    public string Iss { get; init; } = string.Empty; // siempre "https://accounts.google.com"
    public string Sub { get; init; } = string.Empty; // identificador único de usuario
    public string Email { get; init; } = string.Empty;              // email del usuario (si scope=email)

    // Opcionales
    public string? AtHash { get; init; }             // hash del access_token (si aplica)
    public string? Azp { get; init; }                // client_id autorizado
    public bool? EmailVerified { get; init; }        // true si Google verificó el email
    public string? FamilyName { get; init; }         // apellidos
    public string? GivenName { get; init; }          // nombre(s)
    public string? Hd { get; init; }                 // dominio de Google Workspace (si aplica)
    public string? Locale { get; init; }             // ej. "es", "en-US"
    public string? Name { get; init; }               // nombre completo visible
    public string? Nonce { get; init; }              // usado para evitar replay attacks
    public string? Picture { get; init; }            // URL de foto de perfil
    public string? Profile { get; init; }            // URL del perfil público
}