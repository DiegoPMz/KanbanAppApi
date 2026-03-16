using System.Collections.Immutable;
using System.Text;
using FluentResults;
using KanbanAppApi.Data;
using KanbanAppApi.Features.Board.Common;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Features.Board;

public class GetBoardsPaginated
{
    public record struct Query(int Limit, string? Cursor, Guid UserId);

    public record PaginationResponse<TI>(
        ImmutableList<TI> Items,
        string? CurrentCursor,
        string? NextCursor,
        string? PreviousCursor,
        bool HasNextPage,
        bool HasPreviousPage
    );

    interface IQueryHandler
    {
        Task<Result<PaginationResponse<BoardDto>>> HandleAsync(Query query);
    }
    
    public static string EncodeToBase64(string? plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("Input string cannot be null or empty.");

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainBytes);
    }

    // Decode a Base64 string back to plain text
    public static string DecodeFromBase64(string base64Text)
    {
        if (string.IsNullOrEmpty(base64Text))
            throw new ArgumentException("Base64 string cannot be null or empty.");

        try
        {
            byte[] base64Bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(base64Bytes);
        }
        catch (FormatException)
        {
            throw new FormatException("Invalid Base64 string format.");
        }
    }

    
    class QueryHandler(ApplicationContextDb context) : IQueryHandler
    {
        public async Task<Result<PaginationResponse<BoardDto>>> HandleAsync(Query query)
        {
            var limit = query.Limit is < 1 or > 100 
                ? 10 
                : query.Limit;
            
            if (query.Cursor is null)
            {
                var entities = await context.Boards
                    .OrderBy(b => b.Id)
                    .Take(limit)
                    .Select(b => BoardDto.FromEntity(b))
                    .ToListAsync(); 

                var items = entities.ToImmutableList();
                
                var lastEntity = entities.LastOrDefault();
                if (lastEntity is null) return Result.Fail("No items found.");
                
                var currentCursor = EncodeToBase64(lastEntity.Id.ToString());
                var response = new PaginationResponse<BoardDto>(
                    items, 
                    currentCursor, 
                    "", 
                    null, 
                    true, 
                    false
                );

                return response;

            }

            var cursor = EncodeToBase64(query.Cursor);
            if (!Guid.TryParse(cursor, out var boardId)) return Result.Fail("Invalid cursor.");
            
            var entities2 = await context.Boards
                .OrderBy(b => b.Id)
                .Where(b => b.Id > boardId)
                .Take(limit)
                .Select(b => BoardDto.FromEntity(b))
                .ToListAsync(); 

            var items2 = entities2.ToImmutableList();
                
            var lastEntity2 = entities2.LastOrDefault();
            if (lastEntity2 is null) return Result.Fail("No items found.");
                
            var currentCursor2 = EncodeToBase64(lastEntity2.Id.ToString());
            var response2 = new PaginationResponse<BoardDto>(
                items2, 
                currentCursor2, 
                "", 
                query.Cursor, 
                true, 
                true
            );

            return response2;
        }
    }
    
    
}