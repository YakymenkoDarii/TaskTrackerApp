using MediatR;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardCommentsMappers;
using TaskTrackerApp.Domain.Constants;
using TaskTrackerApp.Domain.DTOs.CommentAttachment;
using TaskTrackerApp.Domain.Entities;
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
    private readonly IBlobStorageService _blobService;

    public UpdateCardCommentCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService,
        ICardNotifier cardNotifier,
        IBlobStorageService blobService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _cardNotifier = cardNotifier;
        _blobService = blobService;
    }

    public async Task<Result> Handle(UpdateCardCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId == 0)
        {
            return Result.Failure(AuthErrors.NotAuthenticated);
        }

        var comment = await uow.CardCommentsRepository.GetByIdWithAttachmentsAsync(request.Id);

        if (comment == null)
        {
            return Result.Failure(CommentErrors.NotFound);
        }
        if (comment.CreatedById != currentUserId)
        {
            return Result.Failure(CommentErrors.NotOwner);
        }

        bool hasFileChanges = false;

        if (request.KeepAttachmentIds != null)
        {
            var attachmentsToDelete = comment.Attachments
                .Where(a => !request.KeepAttachmentIds.Contains(a.Id))
                .ToList();

            if (attachmentsToDelete.Any())
            {
                hasFileChanges = true;
                foreach (var attachment in attachmentsToDelete)
                {
                    var blobPath = $"card-{comment.CardId}/comment-{comment.Id}/{attachment.StoredFileName}";

                    await _blobService.DeleteAsync(BlobContainerNames.CommentAttachments, blobPath);

                    comment.Attachments.Remove(attachment);
                }
            }
        }

        if (request.NewAttachments != null && request.NewAttachments.Any())
        {
            hasFileChanges = true;
            foreach (var fileInput in request.NewAttachments)
            {
                var ext = Path.GetExtension(fileInput.FileName);
                var storedName = $"{Guid.NewGuid()}{ext}";
                var blobPath = $"card-{comment.CardId}/comment-{comment.Id}/{storedName}";

                var url = await _blobService.UploadAsync(
                    fileInput.FileContent,
                    BlobContainerNames.CommentAttachments,
                    blobPath,
                    fileInput.ContentType
                );

                comment.Attachments.Add(new CommentAttachment
                {
                    FileName = fileInput.FileName,
                    StoredFileName = storedName,
                    ContentType = fileInput.ContentType,
                    Url = url,
                    Size = fileInput.Size
                });
            }
        }

        comment.ApplyUpdate(request, hasFileChanges);

        await uow.SaveChangesAsync(cancellationToken);

        var attachmentDtos = comment.Attachments.Select(a => new CommentAttachmentDto
        {
            Id = a.Id,
            FileName = a.FileName,
            Url = a.Url,
            ContentType = a.ContentType,
            Size = a.Size
        }).ToList();

        var evt = new CommentUpdatedEvent(
            comment.Id,
            comment.CardId,
            comment.Text,
            comment.UpdatedAt!.Value,
            attachmentDtos
        );

        await _cardNotifier.NotifyCommentUpdatedAsync(evt);

        return Result.Success();
    }
}