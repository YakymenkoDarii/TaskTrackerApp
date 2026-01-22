using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.Events.BoardMember;
public record BoardMemberRoleUpdatedEvent(
    int BoardId,
    int UserId,
    BoardRole NewRole
);