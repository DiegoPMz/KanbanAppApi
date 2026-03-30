using KanbanAppApi.Domain.BoardAggregate;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Data;

public class ApplicationContextDb(DbContextOptions<ApplicationContextDb> options) : DbContext(options)
{
    public DbSet<Board> Boards { get; set; }
    public DbSet<Column> Columns { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContextDb).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}