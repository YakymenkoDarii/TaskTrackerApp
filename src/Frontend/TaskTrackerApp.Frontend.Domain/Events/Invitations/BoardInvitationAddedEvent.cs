namespace TaskTrackerApp.Frontend.Domain.Events.Invitations;
public record BoardInvitationAddedEvent(
    int BoardId,
    int InvitationId,
    string InviteeEmail,
    string Role
);