using KanbanAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Data;

public class ApplicationContextDb(DbContextOptions<ApplicationContextDb> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Column> Columns { get; set; }
    public DbSet<BoardTask> BoardTask { get; set; }
    public DbSet<SubTask> Subtasks { get; set; }
    public DbSet<TokenEntity> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TokenEntity>(entity =>
        {
            entity.HasKey(t => t.Jti);

            entity.Property(t => t.Jti)
                .ValueGeneratedNever();
        });
            
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id); 

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Sub)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}