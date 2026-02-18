using MediatR;
using TaskTrackerApp.Application.Features.CardComments.Commands.CreateCardCommentCommand;
using TaskTrackerApp.Application.HelperMethods;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Constants;
using TaskTrackerApp.Domain.DTOs.CommentAttachment;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Errors.Board;
using TaskTrackerApp.Domain.Events.Comment;
using TaskTrackerApp.Domain.Results;

public class CreateCardCommentCommandHandler : IRequestHandler<CreateCardCommentCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBlobStorageService _blobService;
    private readonly ICardNotifier _notifier;

    public CreateCardCommentCommandHandler(
        IUnitOfWorkFactory uowFactory,
        IBlobStorageService blobService,
        ICardNotifier notifier)
    {
        _uowFactory = uowFactory;
        _blobService = blobService;
        _notifier = notifier;
    }

    public async Task<Result> Handle(CreateCardCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetById(request.CardId);
        var isArchived = await uow.BoardRepository.IsBoardArchivedAsync(card.BoardId);
        if (isArchived)
        {
            return BoardErrors.Archived;
        }

        var boardInfo = await uow.BoardRepository.GetById(card.BoardId);
        var owner = await uow.UserRepository.GetById(boardInfo.CreatedById);

        if (owner != null && !owner.IsPro)
        {
            var ownerBoards = await uow.BoardRepository.GetByCreatorIdAsync(owner.Id);

            var activeBoardIds = ownerBoards
                .OrderByDescending(b => b.LastModified)
                .Take(3)
                .Select(b => b.Id)
                .ToHashSet();

            if (!activeBoardIds.Contains(card.BoardId))
            {
                return Result.Failure(new Error("BoardLocked",
                    "This board is Read-Only because the owner has reached their free plan limit."));
            }
        }

        var comment = new CardComment
        {
            CardId = request.CardId,
            Text = request.Text,
            CreatedById = request.CreatedById,
            CreatedAt = DateTime.UtcNow,
            UpdatedById = request.CreatedById,
            UpdatedAt = DateTime.UtcNow,
            Attachments = new List<CommentAttachment>()
        };

        await uow.CardCommentsRepository.AddAsync(comment);
        await uow.SaveChangesAsync(cancellationToken);

        var cleanHtml = await ImageConverter.UploadEmbeddedImagesAsync(
        comment.Text,
        comment.CardId,
        comment.Id,
        comment.Attachments,
        _blobService
        );

        comment.Text = cleanHtml;

        if (request.Attachments != null && request.Attachments.Any())
        {
            foreach (var attachmentDto in request.Attachments)
            {
                var ext = Path.GetExtension(attachmentDto.FileName);
                var storedName = $"{Guid.NewGuid()}{ext}";
                var blobPath = $"card-{request.CardId}/comment-{comment.Id}/{storedName}";

                var url = await _blobService.UploadAsync(
                    attachmentDto.FileContent,
                    BlobContainerNames.CommentAttachments,
                    blobPath,
                    attachmentDto.ContentType
                );

                attachmentDto.Url = url;
                attachmentDto.StoredFileName = storedName;

                comment.Attachments.Add(new CommentAttachment
                {
                    FileName = attachmentDto.FileName,
                    StoredFileName = storedName,
                    Url = url,
                    ContentType = attachmentDto.ContentType,
                    Size = attachmentDto.Size
                });
            }
        }

        await uow.SaveChangesAsync(cancellationToken);

        var user = await uow.UserRepository.GetById(comment.CreatedById);

        var attachmentDtos = comment.Attachments.Select(a => new CommentAttachmentDto
        {
            Id = a.Id,
            FileName = a.FileName,
            Url = a.Url,
            ContentType = a.ContentType,
            Size = a.Size
        }).ToList();

        var evt = new CommentAddedEvent(
            comment.Id,
            comment.CardId,
            comment.Text,
            user?.Id ?? 0,
            user?.DisplayName ?? "Unknown",
            user?.AvatarUrl,
            comment.CreatedAt,
            attachmentDtos
        );

        await _notifier.NotifyCommentAddedAsync(evt);

        var board = await uow.BoardRepository.GetById(card.BoardId);
        if (board != null)
        {
            board.LastModified = DateTime.UtcNow;
            await uow.BoardRepository.UpdateAsync(board);
        }

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}