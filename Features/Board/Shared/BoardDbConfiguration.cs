using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KanbanAppApi.Features.Board.Shared;

public class BoardDbConfiguration : IEntityTypeConfiguration<Domain.BoardAggregate.Board>
{
    public void Configure(EntityTypeBuilder<Domain.BoardAggregate.Board> builder)
    {
        builder.ToTable("boards");
        builder.HasKey(b => b.Id);
            
        builder.Property(b => b.UserId)
            .HasColumnName("user_id")
            .IsRequired(); 
            
        builder.HasIndex(e => e.UserId);
            
        builder.HasMany(b => b.Columns) 
            .WithOne() 
            .HasForeignKey("board_id") 
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Property(e=> e.Name)
            .HasMaxLength(250)
            .IsRequired();
    }
}