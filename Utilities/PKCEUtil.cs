using System.Security.Cryptography;
using System.Text;

namespace KanbanAppApi.Utilities
{
    public static class PKCEUtil
    {
        public static (string code_challenge, string verifier) Generate(int size = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[size];
            rng.GetBytes(randomBytes);
            var verifier = Base64UrlEncode(randomBytes);

            var buffer = Encoding.UTF8.GetBytes(verifier);
            var hash = SHA256.Create().ComputeHash(buffer);
            var challenge = Base64UrlEncode(hash);

            return (challenge, verifier);
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
          
    }
}
