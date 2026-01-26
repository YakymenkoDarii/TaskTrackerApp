namespace TaskTrackerApp.Frontend.Domain.Events.Invitations;
public record InvitationRespondedEvent(
    string Message,
    bool IsAccepted
);