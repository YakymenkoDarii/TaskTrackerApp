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

    public DbSet<BoardInvitation> BoardInvitations { get; set; }

    public DbSet<CardComment> CardComments { get; set; }

    public DbSet<Label> Labels { get; set; }

    public DbSet<CommentAttachment> CommentAttachments { get; set; }

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
                  .WithMany(u => u.BoardMemberships)
                  .HasForeignKey(bm => bm.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BoardInvitation>()
        .Property(b => b.Role)
        .HasConversion<string>();

        modelBuilder.Entity<CardComment>(entity =>
        {
            entity.HasOne(c => c.CreatedBy)
                  .WithMany()
                  .HasForeignKey(c => c.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Card)
                  .WithMany(card => card.Comments)
                  .HasForeignKey(c => c.CardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasOne(l => l.Board)
                  .WithMany()
                  .HasForeignKey(l => l.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(l => l.Cards)
                  .WithMany(c => c.Labels)
                  .UsingEntity<Dictionary<string, object>>(
                      "CardLabels",

                      right => right.HasOne<Card>()
                                    .WithMany()
                                    .HasForeignKey("CardId")
                                    .OnDelete(DeleteBehavior.Cascade),

                      left => left.HasOne<Label>()
                                  .WithMany()
                                  .HasForeignKey("LabelId")
                                  .OnDelete(DeleteBehavior.Restrict)
                  );
        });

        modelBuilder.Entity<CommentAttachment>(entity =>
        {
            entity.HasOne(a => a.Comment)
                  .WithMany(c => c.Attachments)
                  .HasForeignKey(a => a.CommentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}