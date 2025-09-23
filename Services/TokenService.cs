using KanbanAppApi.Dtos;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KanbanAppApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenValidationParameters _validationParams;
        private readonly JwtSecurityTokenHandler _handler = new();

        public TokenService(IConfiguration configuration)
        {
            var secretKey =configuration["JwtSecretKey"]!;
            _validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "KanbanAppApi",
                ValidateAudience = true,
                ValidAudience = "KanbanApp",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                )
            };
        }

        public string GenerateToken(User user)
        {
            var tokenDescriptor = CreateTokenDescriptor(user, DateTime.UtcNow.AddHours(1));
            var token = _handler.CreateToken(tokenDescriptor);
            return _handler.WriteToken(token);
        }

        public string GenerateRefreshToken(User user)
        {
            var tokenDescriptor = CreateTokenDescriptor(user, DateTime.UtcNow.AddDays(30));
            var token = _handler.CreateToken(tokenDescriptor);
            return _handler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var principal = _handler.ValidateToken(token, _validationParams, out var _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private SecurityTokenDescriptor CreateTokenDescriptor(User user, DateTime expires)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
            };

            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, "Token"),
                Expires = expires,
                SigningCredentials = _validationParams.IssuerSigningKey is not null
                    ? new SigningCredentials(
                        _validationParams.IssuerSigningKey,
                        SecurityAlgorithms.HmacSha256
                    )
                    : null,
                Issuer = _validationParams.ValidIssuer,
                Audience = _validationParams.ValidAudience,
            };
        }
    }
}
