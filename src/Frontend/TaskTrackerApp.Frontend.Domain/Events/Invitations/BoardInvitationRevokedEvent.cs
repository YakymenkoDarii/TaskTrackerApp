namespace TaskTrackerApp.Frontend.Domain.Events.Invitations;
public record BoardInvitationRevokedEvent(
    int BoardId,
    int InvitationId
);