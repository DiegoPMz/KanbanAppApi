using Microsoft.EntityFrameworkCore;

namespace KanbanAppApi.Data
{
    public class ApplicationContextDB(DbContextOptions<ApplicationContextDB> options) : DbContext(options)
    {
        public DbSet<KanbanAppApi.Models.User> Users { get; set; }
        public DbSet<KanbanAppApi.Models.Board> Boards { get; set; }
        public DbSet<KanbanAppApi.Models.Column> Columns { get; set; }
        public DbSet<KanbanAppApi.Models.Task> Tasks { get; set; }
        public DbSet<KanbanAppApi.Models.SubTask> Subtasks { get; set; }
    }
}
