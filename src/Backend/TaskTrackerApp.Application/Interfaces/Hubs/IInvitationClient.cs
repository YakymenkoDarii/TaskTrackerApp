using TaskTrackerApp.Domain.Events.Invitations;

namespace TaskTrackerApp.Application.Interfaces.Hubs;

public interface IInvitationClient
{
    Task ReceiveInvite(InvitationReceivedEvent notification);

    Task RevokeInvite(int invitationId);

    Task InviteResponded(InvitationRespondedEvent notification);

    Task CardAssigned(int cardId, string cardTitle);
}