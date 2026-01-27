using TaskTrackerApp.Application.Features.CardComments.Commands.CreateCardCommentCommand;
using TaskTrackerApp.Application.Features.CardComments.Commands.UpdateCardCommen;
using TaskTrackerApp.Domain.DTOs.CardComment;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Mappers.CardCommentsMappers;

public static class CardCommentMappers
{
    public static CardCommentDto ToDto(this CardComment entity)
    {
        if (entity == null) return null;

        return new CardCommentDto
        {
            Id = entity.Id,
            Text = entity.Text,
            IsEdited = entity.IsEdited,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CreatedById = entity.CreatedById,
            AuthorName = entity.CreatedBy?.DisplayName,
            AuthorAvatarUrl = entity.CreatedBy?.AvatarUrl,
            Attachments = entity.Attachments?
                .Select(a => new TaskTrackerApp.Domain.DTOs.CommentAttachment.CommentAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    Url = a.Url,
                    ContentType = a.ContentType,
                    Size = a.Size
                })
                .ToList() ?? new()
        };
    }

    public static CardComment ToEntity(this CreateCardCommentCommand dto)
    {
        return new CardComment
        {
            CardId = dto.CardId,
            Text = dto.Text,
            CreatedById = dto.CreatedById,
            UpdatedById = dto.CreatedById,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsEdited = false
        };
    }

    public static void ApplyUpdate(this CardComment entity, UpdateCardCommentCommand dto, bool hasFileChanges)
    {
        if (entity.Text != dto.Text || hasFileChanges)
        {
            entity.Text = dto.Text;
            entity.IsEdited = true;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}