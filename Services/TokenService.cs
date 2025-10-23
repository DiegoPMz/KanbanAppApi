using System.Security.Claims;
using KanbanAppApi.Models;
using KanbanAppApi.Repositories;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace KanbanAppApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly JsonWebTokenHandler _tokenHandler = new();
        private readonly ITokenEntityRepository _tokenEntityRepository;
        private readonly IConfiguration _configuration;
        
        public TokenService(IConfiguration configuration, ITokenEntityRepository tokenEntityRepository)
        {
            _tokenEntityRepository = tokenEntityRepository;
            _configuration = configuration;
        }
        
        public static TokenValidationParameters ValidationParameters(
            string issuer, 
            string audience, 
            string secretKey
            ) => new ()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        };
        
        private (SecurityTokenDescriptor tokenDescriptor, Guid tokenJti) CreateTokenDescriptor(User user, DateTime expires)
        {
            var tokenJti = Guid.NewGuid();  
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"]!));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration["JwtAudience"],
                Issuer = _configuration["JwtIssuer"],
                IssuedAt = DateTime.UtcNow,
                Expires = expires,
                SigningCredentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Jti,  tokenJti.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iss, _configuration["JwtIssuer"]!),
                    new Claim(JwtRegisteredClaimNames.Aud, _configuration["JwtAudience"]!),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                ])
            };

            return (tokenDescriptor, tokenJti);
        }
        
        public string GenerateToken(User user)
        {
            var (tokenDescriptor, _) = CreateTokenDescriptor(user, DateTime.UtcNow.AddHours(1));
            return _tokenHandler.CreateToken(tokenDescriptor);
        }

        public async Task<string> GenerateRefreshToken(User user)
        {
            var (tokenDescriptor, tokenJti) = CreateTokenDescriptor(user, DateTime.UtcNow.AddDays(30));
            var refreshToken = _tokenHandler.CreateToken(tokenDescriptor);
            
            await _tokenEntityRepository.StoreTokenAsync(new TokenEntity(tokenJti,user.Id));
            return refreshToken;
        }

        public async Task<(string accessToken, string refreshToken)> CreateAuthTokens(User user)
        {
            var accessToken = GenerateToken(user);
            var refreshToken = await GenerateRefreshToken(user);
            return (accessToken, refreshToken);
        }
        
        public async Task InvalidateRefreshTokenByJtiAsync(Guid tokenJti)
        {
            await _tokenEntityRepository.DeleteTokenByJtiAsync(tokenJti);
        }

        public async Task<TokenValidationResult> ValidateToken(string token)
        {
            return await _tokenHandler.ValidateTokenAsync(token, ValidationParameters(
                    _configuration["JwtIssuer"]!,
                    _configuration["JwtAudience"]!,
                    _configuration["JwtSecretKey"]!
                )
            );
        }
    }
}
