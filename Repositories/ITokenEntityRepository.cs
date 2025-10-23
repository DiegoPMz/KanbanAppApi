using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface ITokenEntityRepository
    {
        Task StoreTokenAsync(TokenEntity token);
        Task DeleteTokenByJtiAsync(Guid refreshTokenJti);
    }
}
