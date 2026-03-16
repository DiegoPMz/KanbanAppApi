using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Data;

public class ApplicationContextDb(DbContextOptions<ApplicationContextDb> options) : DbContext(options)
{
    public DbSet<Core.Entities.Board> Boards { get; set; }
    public DbSet<Core.Entities.Column> Columns { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Core.Entities.Board>(entity =>
        {
            entity.ToTable("boards");
            entity.HasKey(b => b.Id);
            
            entity.HasMany(b => b.Columns)
                .WithOne()
                .HasForeignKey("board_id")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(b => b.UserId)
                .HasColumnName("user_id")
                .IsRequired(); 
            
            entity.HasIndex("UserId");
            
            entity.Property(e=> e.Name)
                .HasMaxLength(250)
                .IsRequired();
        });
        
        modelBuilder.Entity<Core.Entities.Column>(entity =>
        {
            entity.ToTable("columns");
            entity.HasKey(e => e.Id);
            
            entity.Property(e=> e.Name)
                .HasMaxLength(250)
                .IsRequired();
            
            entity.Property(c => c.Order)
                .IsRequired();

            entity.Property(e => e.Color)
                .HasMaxLength(20);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}