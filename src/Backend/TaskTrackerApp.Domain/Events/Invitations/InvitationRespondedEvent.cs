namespace TaskTrackerApp.Domain.Events.Invitations;
public record InvitationRespondedEvent(
    string Message,
    bool IsAccepted
);