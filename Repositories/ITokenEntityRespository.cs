using KanbanAppApi.Models;

namespace KanbanAppApi.Repositories
{
    public interface ITokenEntityRespository
    {
        Task StoreTokenAsync(TokenEntity token);
        Task DeleteTokenByJtiAsync(Guid refreshTokenJti);
    }
}
