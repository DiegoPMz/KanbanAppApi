using KanbanAppApi.Common.Domain.BoardAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KanbanAppApi.Features.Board.Shared;

public class ColumnDbConfiguration : IEntityTypeConfiguration<Column>
{
    public void Configure(EntityTypeBuilder<Column> builder)
    {
        builder.ToTable("columns");
        
        builder.HasKey(e => e.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever();
            
        builder.Property(e=> e.Name)
            .HasMaxLength(250)
            .IsRequired();
            
        builder.Property(c => c.Order)
            .IsRequired();

        builder.Property(e => e.Color)
            .HasMaxLength(20);
        
        builder.Property(c => c.TaskIds)
            .HasColumnName("task_ids")
            .HasColumnType("uuid[]")
            .HasField("_taskIds") 
            .HasDefaultValueSql("'{}'")
            .IsRequired();
    }
}