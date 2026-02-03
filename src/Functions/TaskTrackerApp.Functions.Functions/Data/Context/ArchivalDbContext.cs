using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Functions.Functions.Data.Entities;

namespace TaskTrackerApp.Functions.Functions.Data.Context;

public class ArchivalDbContext : DbContext
{
    public ArchivalDbContext(DbContextOptions<ArchivalDbContext> options) : base(options)
    {
    }

    public DbSet<Board> Boards { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Column> Columns { get; set; }

    public DbSet<Card> Cards { get; set; }

    public DbSet<Label> Labels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.ToTable("Boards");
            entity.HasKey(b => b.Id);

            entity.HasOne(b => b.CreatedBy)
                  .WithMany()
                  .HasForeignKey("CreatedById");

            entity.HasMany(b => b.Columns)
                  .WithOne(c => c.Board)
                  .HasForeignKey(c => c.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(b => b.Members)
                  .WithOne(m => m.Board)
                  .HasForeignKey(m => m.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(b => b.Labels)
                  .WithOne(l => l.Board)
                  .HasForeignKey(l => l.BoardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BoardMember>(entity =>
        {
            entity.ToTable("BoardMembers");

            entity.Property(bm => bm.Role).HasConversion<string>();

            entity.HasOne(bm => bm.User)
                  .WithMany()
                  .HasForeignKey(bm => bm.UserId);
        });

        modelBuilder.Entity<Column>(entity =>
        {
            entity.ToTable("Columns");

            entity.HasMany(c => c.Cards)
                  .WithOne(c => c.Column)
                  .HasForeignKey(c => c.ColumnId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.ToTable("Cards");

            entity.Property(c => c.Priority).HasConversion<string>();

            entity.HasMany(c => c.Comments)
                  .WithOne(com => com.Card)
                  .HasForeignKey(com => com.CardId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.AssigneeUser)
                  .WithMany()
                  .HasForeignKey(c => c.AssigneeId);

            entity.HasMany(c => c.Labels)
                  .WithMany(l => l.Cards)
                  .UsingEntity<Dictionary<string, object>>(
                      "CardLabels",
                      right => right.HasOne<Label>().WithMany().HasForeignKey("LabelId"),
                      left => left.HasOne<Card>().WithMany().HasForeignKey("CardId")
                  );
        });

        modelBuilder.Entity<CardComment>(entity =>
        {
            entity.ToTable("CardComments");

            entity.HasOne(c => c.CreatedBy)
                  .WithMany()
                  .HasForeignKey(c => c.CreatedById);

            entity.HasMany(c => c.Attachments)
                  .WithOne(a => a.Comment)
                  .HasForeignKey(a => a.CommentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CommentAttachment>(entity =>
        {
            entity.ToTable("CommentAttachments");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.ToTable("Labels");
        });
    }
}