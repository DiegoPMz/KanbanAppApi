using FluentResults;

namespace KanbanAppApi.Core.Entities;

public class Column
{
    public Guid Id { get;  private init; }
    public string Name { get; private set; }
    public int Order { get;  private set; }
    public string Color { get;  private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }
    
    public Column (Guid id, string name, int order, string color = "#0387")
    {
        Id = id;
        Name = name;
        Order = order;
        Color = color;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string? name, string? color)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (!string.IsNullOrWhiteSpace(color)) Color = color;
        UpdatedAt = DateTime.UtcNow;
    }
}