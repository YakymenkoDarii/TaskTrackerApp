namespace TaskTrackerApp.Domain.Events.BoardMember;
public record BoardMemberAddedEvent(
    int BoardId,
    int UserId,
    string Name,
    string Role,
    string? AvatarUrl
);