using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardCommentsMappers;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Errors.CardComments;
using TaskTrackerApp.Domain.Events.Comment;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.UpdateCardCommen;

public class UpdateCardCommentCommandHandler : IRequestHandler<UpdateCardCommentCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICardNotifier _cardNotifier;

    public UpdateCardCommentCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService, ICardNotifier cardNotifier)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _cardNotifier = cardNotifier;
    }

    public async Task<Result> Handle(UpdateCardCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId == 0)
        {
            return Result.Failure(AuthErrors.NotAuthenticated);
        }

        var comment = await uow.CardCommentsRepository.GetById(request.Id);

        if (comment == null)
        {
            return Result.Failure(CommentErrors.NotFound);
        }
        if (comment.CreatedById != currentUserId)
        {
            return Result.Failure(CommentErrors.NotOwner);
        }

        comment!.ApplyUpdate(request);

        await uow.SaveChangesAsync(cancellationToken);

        var evt = new CommentUpdatedEvent(comment.Id, comment.CardId, comment.Text, comment.UpdatedAt.Value);
        await _cardNotifier.NotifyCommentUpdatedAsync(evt);

        return Result.Success();
    }
}