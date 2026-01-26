using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardCommentsMappers;
using TaskTrackerApp.Domain.Events.Comment;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.CreateCardCommentCommand;

public class CreateCardCommentCommandHandler : IRequestHandler<CreateCardCommentCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICardNotifier _notifier;

    public CreateCardCommentCommandHandler(IUnitOfWorkFactory uowFactory, ICardNotifier notifier)
    {
        _uowFactory = uowFactory;
        _notifier = notifier;
    }

    public async Task<Result> Handle(CreateCardCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var comment = request!.ToEntity();

        await uow.CardCommentsRepository.AddAsync(comment);
        await uow.SaveChangesAsync(cancellationToken);

        var user = await uow.UserRepository.GetById(comment.CreatedById);

        string authorName = user?.DisplayName ?? user?.Email ?? "Unknown";
        string? authorAvatar = user?.AvatarUrl;

        var evt = new CommentAddedEvent(
            comment.Id,
            comment.CardId,
            comment.Text,
            user.Id,
            authorName,
            authorAvatar,
            comment.CreatedAt
        );

        await _notifier.NotifyCommentAddedAsync(evt);

        return Result.Success();
    }
}