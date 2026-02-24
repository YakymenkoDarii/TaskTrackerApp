using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Application.Mappers.BoardMappers;

public static class BoardMapper
{
    public static BoardDto MapToDto(Board board, bool isLocked, bool isStarred, BoardThemeColor themeColor)
    {
        return new BoardDto
        {
            Id = board.Id,
            Title = board.Title,
            Description = board.Description,
            LastModified = board.LastModified,
            CreatedById = board.CreatedById,
            IsLocked = isLocked,
            IsStarred = isStarred,
            ThemeColor = themeColor,
            Members = board.Members?.Select(m => new BoardMemberAvatarDto
            {
                UserId = m.User.Id,
                DisplayName = m.User.DisplayName ?? "Unknown",
                AvatarUrl = m.User.AvatarUrl
            }).ToList() ?? new List<BoardMemberAvatarDto>()
        };
    }
}