using TaskTrackerApp.Frontend.Domain.Events.Invitations;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Hubs;

public interface IInvitationClient
{
    Task ReceiveInvite(InvitationReceivedEvent notification);

    Task RevokeInvite(int invitationId);

    Task InviteResponded(InvitationRespondedEvent notification);

    Task CardAssigned(int cardId, string cardTitle);
}