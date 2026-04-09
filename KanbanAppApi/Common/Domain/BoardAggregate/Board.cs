using ErrorOr;

namespace KanbanAppApi.Common.Domain.BoardAggregate;

public record ColumnOrderInput(Guid Id, int NewOrder);

public class Board
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    public Guid UserId { get; private set; }

    private readonly List<Column> _columns  = [];
    public IReadOnlyCollection<Column> Columns => _columns.AsReadOnly();
    
    private Board(){ }
    private Board(string name, Guid userId)
    {
        Name = name;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static ErrorOr<Board> Create(string name, Guid userId)
    {
        if (name.Trim().Length < 1 || name.Length > 250)
        {
            return BoardErrors.InvalidName(name);
        }
        
        return new Board(name,  userId);
    }
    
    public ErrorOr<Column> AddColumn(string name, string? color)
    {
        if (_columns.Any(c => c.Name == name))
            return BoardErrors.NameAlreadyExists(name);

        if (_columns.Count >= 10)
            return BoardErrors.LimitReached(10);

        var newColumn = new Column(name, _columns.Count + 1, color);
        
        _columns.Add(newColumn);
       return newColumn;
    }
    
    public ErrorOr<Column> EditColumn(Guid id, string? name, string? color)
    {
        var column = _columns.FirstOrDefault(c => c.Id == id);
        if (column is null) return BoardErrors.ColumnNotFound(id.ToString());

        if (name is not null && name != column.Name)
        {
            if (_columns.Any(c => c.Name == name && c.Id != id)) 
                return BoardErrors.NameAlreadyExists(name);
        }
    
        column.Update(name, color);
        return column;
    }
    
    public ErrorOr<Deleted> RemoveColumn(Guid id)
    {
        var column = _columns.FirstOrDefault(c => c.Id == id);
        if (column is null) return BoardErrors.ColumnNotFound(id.ToString());
        
        _columns.Remove(column);
        return Result.Deleted;
    }
    
    public ErrorOr<Updated> ReorderColumns(List<ColumnOrderInput> newOrders)
    {
        if (newOrders.Count != _columns.Count)
            return BoardErrors.InvalidColumnCount;

        var columnIds = _columns.Select(c => c.Id).ToList();
        
        if (newOrders.Any(o => !columnIds.Contains(o.Id)))
            return BoardErrors.ColumnNotFoundInBoard;
        
        if (newOrders.Select(o => o.NewOrder).Distinct().Count() != newOrders.Count)
            return BoardErrors.DuplicateColumnOrder;

        foreach (var input in newOrders)
        {
            var column = _columns.First(c => c.Id == input.Id);
            column.UpdateOrder(input.NewOrder); 
        }

        return Result.Updated;
    }
    
}
