using MediatR;
using TaskTrackerApp.Domain.DTOs.CommentAttachment;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.UpdateCardCommen;

public class UpdateCardCommentCommand : IRequest<Result>
{
    public int Id { get; set; }

    public string Text { get; set; }

    public List<int>? KeepAttachmentIds { get; set; }

    public List<AttachmentInputDto>? NewAttachments { get; set; } = new();
}