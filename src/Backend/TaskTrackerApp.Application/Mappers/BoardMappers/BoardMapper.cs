using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Mappers.BoardMappers;

public static class BoardMapper
{
    public static BoardDto MapToDto(Board board, bool isLocked)
    {
        return new BoardDto
        {
            Id = board.Id,
            Title = board.Title,
            Description = board.Description,
            LastModified = board.LastModified,
            CreatedById = board.CreatedById,
            IsLocked = isLocked
        };
    }
}