using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Functions.Functions.Data.Context;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Attachment;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Board;
using TaskTrackerApp.Functions.Functions.Data.Dtos.BoardMember;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Card;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Column;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Comment;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Label;
using TaskTrackerApp.Functions.Functions.Interfaces.Repositories;

namespace TaskTrackerApp.Functions.Functions.Data.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly ArchivalDbContext _context;

    public BoardRepository(ArchivalDbContext context)
    {
        _context = context;
    }

    public async Task<BoardExportDto?> GetFullBoardAsync(int boardId)
    {
        return await _context.Boards
            .AsNoTracking()
            .Where(b => b.Id == boardId)
            .Select(b => new BoardExportDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                CreatedById = EF.Property<int>(b, "CreatedById"),

                Members = b.Members.Select(m => new BoardMemberDto
                {
                    UserId = m.UserId,
                    Role = m.Role.ToString()
                }).ToList(),

                Labels = b.Labels.Select(l => new LabelDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Color = l.Color
                }).ToList(),

                Columns = b.Columns.Select(c => new ColumnDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Position = c.Position,
                    Cards = c.Cards.Select(card => new CardDto
                    {
                        Title = card.Title,
                        Description = card.Description,
                        Position = card.Position,
                        IsCompleted = card.IsCompleted,
                        Priority = card.Priority.ToString(),
                        DueDate = card.DueDate,

                        AssigneeId = card.AssigneeId,

                        LabelIds = card.Labels.Select(l => l.Id).ToList(),

                        Comments = card.Comments.Select(com => new CommentDto
                        {
                            Text = com.Text,
                            CreatedById = com.CreatedById,
                            CreatedAt = com.CreatedAt,
                            Attachments = com.Attachments.Select(a => new AttachmentDto
                            {
                                FileName = a.FileName,
                                StoredFileName = a.StoredFileName,
                                ContentType = a.ContentType,
                                Url = a.Url
                            }).ToList()
                        }).ToList()
                    }).OrderBy(card => card.Position).ToList()
                }).OrderBy(c => c.Position).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(int boardId)
    {
        var board = _context.Boards.Where(b => b.Id == boardId).FirstOrDefault();

        if (board != null)
        {
            _context.Boards.Remove(board);
        }

        await _context.SaveChangesAsync();
        return;
    }
}