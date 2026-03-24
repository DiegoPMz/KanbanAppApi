using System.Security.Claims;
using System.Text;
using FluentResults;
using FluentValidation;
using KanbanAppApi.Common.Extensions;
using KanbanAppApi.Common.Http;
using KanbanAppApi.Data;
using KanbanAppApi.Features.Board.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Error = ErrorOr.Error;

namespace KanbanAppApi.Features.Board;

public sealed class GetBoardsPaginated
{
    public record struct Query(int Limit, string? Cursor, Guid UserId);

    public record struct QueryParameters(int Limit = 10, string? Cursor = null);

    public record PaginationResponse<TI>(
        List<TI> Items,
        string? CurrentCursor,
        string? NextCursor,
        bool HasNextPage,
        bool HasPreviousPage
    );

    public interface IQueryHandler
    {
        Task<Result<PaginationResponse<BoardDto>>> HandleAsync(Query query);
    }

    private static string? SafeEncode(string? text) =>
        string.IsNullOrEmpty(text) ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

    private static string? SafeDecode(string? base64)
    {
        if (string.IsNullOrEmpty(base64)) return null;
        try {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }
        catch { return null; }
    }

    private static bool IsBase64String(string s)
    {
        var buffer = new Span<byte>(new byte[s.Length]);
        return Convert.TryFromBase64String(s, buffer, out _);
    }

    public class QueryHandler(ApplicationContextDb context) : IQueryHandler
    {
        public async Task<Result<PaginationResponse<BoardDto>>> HandleAsync(Query query)
        {
            var queryable = context.Boards
                .AsNoTracking()
                .Where(b => b.UserId == query.UserId)
                .OrderBy(b => b.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.Cursor))
            {
                var decoded = SafeDecode(query.Cursor);
                if (!Guid.TryParse(decoded, out var cursorValue))
                    return Result.Fail("Invalid cursor format.");

                queryable = queryable.Where(b => b.Id > cursorValue);
            }

            var itemsWithExtra = await queryable
                .Take(query.Limit + 1)
                .Select(b => BoardDto.FromEntity(b))
                .ToListAsync();

            var hasNextPage = itemsWithExtra.Count > query.Limit;
            var results = itemsWithExtra.Take(query.Limit).ToList();

            var firstId = results.FirstOrDefault()?.Id.ToString();
            var lastId = results.LastOrDefault()?.Id.ToString();
            
            return new PaginationResponse<BoardDto>(
                Items: results,
                CurrentCursor: SafeEncode(firstId),
                NextCursor: hasNextPage ? SafeEncode(lastId) : null,
                HasNextPage: hasNextPage,
                HasPreviousPage: !string.IsNullOrEmpty(query.Cursor)
            );
        }
    }

    public class Validator : AbstractValidator<QueryParameters>
    {
        public Validator()
        {
            RuleFor(q => q.Limit)
                .InclusiveBetween(1, 100);

            RuleFor(q => q.Cursor)
                .Custom((cursor, context) =>
                {
                    if (cursor == null) return;
                    if (!IsBase64String(cursor))
                    {
                        context.AddFailure("Cursor", "Invalid cursor format.");
                    }
                });
        }
    }

    public static class GetBoardPaginatedEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("api/boards",  async Task<Results<Ok<PaginationResponse<BoardDto>>, ProblemHttpResult, ValidationProblem>> (
                [AsParameters] QueryParameters queryParameters,
                IQueryHandler handler,
                ClaimsPrincipal user
            ) =>
            {
                if (user.GetUserId() is not { } userId) return ApiErrorHandler.Problem(Error.Unauthorized());
                
                var validation = await new Validator().ValidateAsync(queryParameters);
                if (!validation.IsValid) return TypedResults.ValidationProblem(validation.ToDictionary());

                var result = await handler.HandleAsync(new Query(queryParameters.Limit, queryParameters.Cursor, userId));

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : TypedResults.Problem();
            }).RequireAuthorization();
        }
    }

}
