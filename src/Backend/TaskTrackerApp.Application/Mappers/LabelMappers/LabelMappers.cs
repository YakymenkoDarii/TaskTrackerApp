using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Mappers.LabelMappers;

public static class LabelMappers
{
    public static LabelDto ToDto(this Label entity)
    {
        if (entity == null) return null;

        return new LabelDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Color = entity.Color
        };
    }

    public static Label ToEntity(this CreateLabelDto dto)
    {
        return new Label
        {
            Name = dto.Name,
            Color = dto.Color,
            BoardId = dto.BoardId,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyUpdate(this Label entity, LabelDto dto)
    {
        entity.Name = dto.Name;
        entity.Color = dto.Color;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}