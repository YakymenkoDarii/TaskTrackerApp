namespace TaskTrackerApp.Frontend.Domain.Events.BoardMember;
public record BoardMemberRoleUpdatedEvent(
    int BoardId,
    int UserId,
    string NewRole
);