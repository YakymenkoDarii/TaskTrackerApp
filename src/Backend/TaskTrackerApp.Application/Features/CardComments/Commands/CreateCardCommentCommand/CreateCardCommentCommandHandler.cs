using MediatR;
using TaskTrackerApp.Application.Features.CardComments.Commands.CreateCardCommentCommand;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Constants;
using TaskTrackerApp.Domain.DTOs.CommentAttachment;
using TaskTrackerApp.Domain.Entities;
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

            await uow.SaveChangesAsync(cancellationToken);
        }

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

        return Result.Success();
    }
}