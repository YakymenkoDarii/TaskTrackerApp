using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.DTOs.CardComment;
using TaskTrackerApp.Domain.DTOs.Column;
using TaskTrackerApp.Domain.DTOs.CommentAttachment;
using TaskTrackerApp.Domain.DTOs.Labels;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetFullBoardDetails;

public class GetFullBoardDetailsQueryHandler : IRequestHandler<GetFullBoardDetailsQuery, BoardExportDto>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetFullBoardDetailsQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<BoardExportDto> Handle(GetFullBoardDetailsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var board = await uow.BoardRepository.GetFullBoardDetails(request.BoardId);

        if (board == null) return null;

        return new BoardExportDto
        {
            Id = board.Id,
            Title = board.Title,
            Description = board.Description,
            IsArchived = board.IsArchived,
            LastTimeOpenned = board.LastTimeOpenned,

            Members = board.Members.Select(m => new BoardMemberDto
            {
                UserId = m.UserId,
                Name = m.User?.DisplayName ?? "Unknown Member",
                Role = m.Role.ToString(),
                AvatarUrl = m.User?.AvatarUrl
            }).ToList(),

            Columns = board.Columns.Select(c => new ColumnExportDto
            {
                Id = c.Id,
                Title = c.Title,
                Position = c.Position,
                Cards = c.Cards.Select(card => new CardExportDto
                {
                    Id = card.Id,
                    Title = card.Title,
                    Description = card.Description,
                    Position = card.Position,
                    Labels = card.Labels.Select(l => new LabelDto
                    {
                        Name = l.Name,
                        Color = l.Color
                    }).ToList(),
                    Comments = card.Comments.Select(com => new CardCommentExportDto
                    {
                        Text = com.Text,
                        AuthorName = com.CreatedBy?.DisplayName ?? "Unknown User",
                        IsEdited = com.IsEdited,
                        AuthorAvatarUrl = com.CreatedBy?.AvatarUrl,
                        Attachments = com.Attachments.Select(attachment => new AttachmentExportDto
                        {
                            FileName = attachment.FileName,
                            ContentType = attachment.ContentType,
                            Size = attachment.Size,
                            Url = attachment.Url
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }
}