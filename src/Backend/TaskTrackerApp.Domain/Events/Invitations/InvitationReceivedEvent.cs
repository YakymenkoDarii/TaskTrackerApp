namespace TaskTrackerApp.Domain.Events.Invitations;
public record InvitationReceivedEvent(
    int InvitationId,
    int BoardId,
    string Sender,
    string BoardName
);