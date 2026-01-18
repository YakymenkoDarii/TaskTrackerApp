using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Application.Mappers.CardMappers;

public static class CardMappers
{
    public static CardDto ToDto(this Card entity)
    {
        if (entity == null) return null;

        return new CardDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueDate,
            AssigneeId = entity.AssigneeId,
            CreatedAt = entity.CreatedAt,
            IsCompleted = entity.IsCompleted,
            Position = entity.Position,
            ColumnId = entity.ColumnId,
            BoardId = entity.BoardId,
            Priority = entity.Priority,
            Labels = entity.Labels != null
                ? entity.Labels.Select(l => new LabelDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Color = l.Color
                }).ToList()
                : new List<LabelDto>()
        };
    }

    public static UpcomingCardDto ToUpcomingDto(this Card entity)
    {
        if (entity == null) return null;

        return new UpcomingCardDto
        {
            Id = entity.Id,
            Title = entity.Title,
            DueDate = entity.DueDate,
            IsCompleted = entity.IsCompleted,
            BoardId = entity.BoardId,
            Priority = entity.Priority,

            BoardTitle = entity.Board?.Title ?? "Unknown Board",
            ColumnTitle = entity.Column?.Title ?? "Unknown List"
        };
    }

    public static Card ToEntity(this CreateCardDto dto)
    {
        return new Card
        {
            Title = dto.Title,
            Description = dto.Description ?? string.Empty,
            DueDate = dto.DueDate,
            ColumnId = dto.ColumnId,
            BoardId = dto.BoardId,
            AssigneeId = dto.AssigneeId,
            CreatedById = dto.CreatedById,
            UpdatedById = dto.CreatedById,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Priority = CardPriority.Low,
            IsCompleted = false,
            IsArchived = false
        };
    }

    public static void ApplyUpdate(this Card entity, UpdateCardDto dto)
    {
        entity.Title = dto.Title;
        entity.Description = dto.Description ?? string.Empty;
        entity.DueDate = dto.DueDate;
        entity.ColumnId = dto.ColumnId;
        entity.BoardId = dto.BoardId;
        entity.AssigneeId = dto.AssigneeId;
        entity.UpdatedById = dto.UpdatedById;
        entity.Position = dto.Position;
        entity.IsCompleted = dto.IsCompleted;
        entity.Priority = dto.Priority;

        entity.UpdatedAt = DateTime.UtcNow;
    }
}