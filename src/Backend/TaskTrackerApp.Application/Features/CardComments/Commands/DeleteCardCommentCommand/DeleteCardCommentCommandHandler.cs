using MediatR;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Constants;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Errors.CardComments;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.DeleteCardCommentCommand;

public class DeleteCardCommentCommandHandler : IRequestHandler<DeleteCardCommentCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICardNotifier _cardNotifier;
    private readonly IBlobStorageService _blobService;

    public DeleteCardCommentCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService, ICardNotifier cardNotifier, IBlobStorageService blobService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _cardNotifier = cardNotifier;
        _blobService = blobService;
    }

    public async Task<Result> Handle(DeleteCardCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId == 0)
            return Result.Failure(AuthErrors.NotAuthenticated);

        var comment = await uow.CardCommentsRepository.GetByIdWithAttachmentsAsync(request.Id);

        if (comment == null)
        {
            return Result.Failure(CommentErrors.NotFound);
        }

        if (comment.CreatedById != currentUserId)
        {
            return Result.Failure(CommentErrors.NotOwner);
        }

        var filesToDelete = comment.Attachments.Select(a => new
        {
            a.StoredFileName,
            CommentId = comment.Id,
            CardId = comment.CardId
        }).ToList();

        await uow.CardCommentsRepository.DeleteAsync(comment.Id);
        await uow.SaveChangesAsync(cancellationToken);

        foreach (var file in filesToDelete)
        {
            var blobPath = $"card-{file.CardId}/comment-{file.CommentId}/{file.StoredFileName}";

            try
            {
                await _blobService.DeleteAsync(BlobContainerNames.CommentAttachments, blobPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete blob {blobPath}: {ex.Message}");
            }
        }

        await _cardNotifier.NotifyCommentDeletedAsync(comment.Id, comment.CardId);

        return Result.Success();
    }
}