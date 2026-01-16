using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Errors.CardComments;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.DeleteCardCommentCommand;

public class DeleteCardCommentCommandHandler : IRequestHandler<DeleteCardCommentCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCardCommentCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteCardCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId == 0)
            return Result.Failure(AuthErrors.NotAuthenticated);

        var comment = await uow.CardCommentsRepository.GetById(request.Id);

        if (comment == null)
        {
            return Result.Failure(CommentErrors.NotFound);
        }

        if (comment.CreatedById != currentUserId)
        {
            return Result.Failure(CommentErrors.NotOwner);
        }

        await uow.CardCommentsRepository.DeleteAsync(comment.Id);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}