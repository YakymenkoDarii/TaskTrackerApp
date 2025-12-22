using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Persistence.Contexts;

public class TaskTrackerDbContext : DbContext
{
    public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardMember> BoardMembers { get; set; }
    public DbSet<Column> Columns { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasOne(b => b.CreatedBy)
                  .WithMany(u => u.CreatedBoards)
                  .HasForeignKey(b => b.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.UpdatedBy)
                  .WithMany()
                  .HasForeignKey(b => b.UpdatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(b => b.Columns).WithOne(c => c.Board).HasForeignKey(c => c.BoardId);
            entity.HasMany(b => b.Cards).WithOne(c => c.Board).HasForeignKey(c => c.BoardId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Column>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasOne(c => c.Board)
                  .WithMany(b => b.Columns)
                  .HasForeignKey(c => c.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasOne(c => c.CreatedBy)
                  .WithMany()
                  .HasForeignKey(c => c.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.UpdatedBy)
                  .WithMany()
                  .HasForeignKey(c => c.UpdatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.AssigneeUser)
                  .WithMany(u => u.AssignedTasks)
                  .HasForeignKey(c => c.AssigneeId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(c => c.Role)
                .HasConversion<string>();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BoardMember>(entity =>
        {
            entity.HasIndex(bm => new { bm.BoardId, bm.UserId })
                  .IsUnique();

            entity.Property(bm => bm.Role)
                  .HasConversion<string>();

            entity.HasOne(bm => bm.Board)
              .WithMany(b => b.Members)
              .HasForeignKey(bm => bm.BoardId)
              .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(bm => bm.User)
              .WithMany()
              .HasForeignKey(bm => bm.UserId)
              .OnDelete(DeleteBehavior.Restrict);
        });
    }
}