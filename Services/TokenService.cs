using KanbanAppApi.Models;
using KanbanAppApi.Repositories;
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
        private readonly ITokenEntityRespository _tokenEntityRespository;

        public TokenService(IConfiguration configuration, ITokenEntityRespository tokenEntityRespository)
        {
            _tokenEntityRespository = tokenEntityRespository;

            var secretKey = configuration["JwtSecretKey"]!;
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
            var (tokenDescriptor, tokenJti) = CreateTokenDescriptor(user, DateTime.UtcNow.AddHours(1));
            var token = _handler.CreateToken(tokenDescriptor);
            return _handler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken(User user)
        {
            var (tokenDescriptor, tokenJti) = CreateTokenDescriptor(user, DateTime.UtcNow.AddDays(30));
            var token = _handler.CreateToken(tokenDescriptor);

            TokenEntity refreshToken = new()
            {
                Jti = tokenJti,
                UserId = user.Id,
            };

            await _tokenEntityRespository.StoreTokenAsync(refreshToken);
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

        private (SecurityTokenDescriptor tokenDescriptor, Guid tokenJti) CreateTokenDescriptor(User user, DateTime expires)
        {
            var tokenJti = Guid.NewGuid();  

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, tokenJti.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
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


            return (tokenDescriptor, tokenJti);
        }

        public async Task InvalidateRefreshTokenByJtiAsync(Guid tokenJti)
        {
            await _tokenEntityRespository.DeleteTokenByJtiAsync(tokenJti);
        }
    }
}
