using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Persistence.Contexts
{
    public class TaskTrackerDbContext : DbContext
    {
        public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<BoardMembers> BoardMembers { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BoardMembers>(entity =>
            {
                entity.HasKey(bm => bm.Id);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(bm => bm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Board>()
                    .WithMany()
                    .HasForeignKey(bm => bm.BoardId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasOne<User>()
                    .WithMany(u => u.CreatedBoards)
                    .HasForeignKey(b => b.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(b => b.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Column>(entity =>
            {
                entity.HasOne<Board>()
                    .WithMany(b => b.Columns)
                    .HasForeignKey(c => c.BoardId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasOne<Column>()
                    .WithMany(c => c.Cards)
                    .HasForeignKey(c => c.ColumnId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Board>()
                    .WithMany()
                    .HasForeignKey(c => c.BoardId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<User>()
                    .WithMany(u => u.AssignedTasks)
                    .HasForeignKey(c => c.AssigneeId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}