using FluentResults;

namespace KanbanAppApi.Core.Entities;

public class Board
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    public Guid UserId { get; private set; }

    private readonly List<Column> _columns = [];
    public IReadOnlyCollection<Column> Columns => _columns.AsReadOnly();
    
    protected Board(){ }
    
    public Board(string name, Guid userId)
    {
        Name = name;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public Result AddColumn(string name, string? color)
    {
        if (Columns.Any(c => c.Name == name))
            return Result.Fail("Already exists column with that name.");

        if (Columns.Count >= 10)
            return Result.Fail("A Board cannot have more than 10 columns.");

        var newColumn = new Column(Guid.NewGuid(), name, _columns.Count + 1);
        
        _columns.Add(newColumn);
       return  Result.Ok();
    }
    
    public Result EditColumn(Guid id, string? name, string? color)
    {
        var column = Columns.FirstOrDefault(c => c.Id == id);
        if (column is null) return  Result.Fail("Column not found.");
        
        column.Update(name, color);
        return Result.Ok();
    }
    
    public Result DeleteColumn(Guid id)
    {
        var column = Columns.FirstOrDefault(c => c.Id == id);
        if (column is null) return  Result.Fail("Column not found.");
        
        _columns.Remove(column);
        
        return Result.Ok();
    }
}
