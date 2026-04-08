using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace KanbanAppApi.Features.Auth.Infrastructure;

public record struct PkceCodesGenerated(string CodeChallenge, string CodeVerifier);

public static class PkceUtilities
{
    public static PkceCodesGenerated  GenerateCodes(int size = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[size];
        rng.GetBytes(randomBytes);
        var verifier = Base64UrlEncoder.Encode(randomBytes);
        var buffer = Encoding.UTF8.GetBytes(verifier);
        var hash = SHA256.HashData(buffer);
        var challenge = Base64UrlEncoder.Encode(hash);

        return new PkceCodesGenerated(challenge, verifier);
    }

    private static string Base64UrlEncode(byte[] data)
    {
        return Convert.ToBase64String(data)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}