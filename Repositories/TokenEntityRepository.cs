using KanbanAppApi.Data;
using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Repositories;

public class TokenEntityRepository : ITokenEntityRepository
{
    private readonly ApplicationContextDb _context;

    public TokenEntityRepository(ApplicationContextDb context)
    {
        _context = context;
    }

    public async Task DeleteTokenByJtiAsync(Guid refreshTokenJti)
    {
        await _context.Tokens
            .Where(t => t.Jti == refreshTokenJti)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
    }

    public async Task StoreTokenAsync(TokenEntity token)
    {
        await _context.Tokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }
}