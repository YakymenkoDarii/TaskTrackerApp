namespace TaskTrackerApp.Frontend.Domain.Events.BoardMember;
public record BoardMemberRemovedEvent(
    int BoardId,
    int UserId
);