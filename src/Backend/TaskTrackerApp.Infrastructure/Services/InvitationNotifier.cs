using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Interfaces.Hubs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Events.Invitations;
using TaskTrackerApp.Infrastructure.Hubs;

namespace TaskTrackerApp.Infrastructure.Services;

public class InvitationNotifier : IInvitationNotifier
{
    private readonly IHubContext<InvitationHub, IInvitationClient> _hubContext;

    public InvitationNotifier(IHubContext<InvitationHub, IInvitationClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyUserReceivedInviteAsync(int userId, int invitationId, int boardId, string senderName, string boardName)
    {
        var payload = new InvitationReceivedEvent(invitationId, boardId, senderName, boardName);

        await _hubContext.Clients.User(userId.ToString()).ReceiveInvite(payload);
    }

    public async Task NotifyUserInviteRevokedAsync(int userId, int invitationId)
    {
        await _hubContext.Clients.User(userId.ToString()).RevokeInvite(invitationId);
    }

    public async Task NotifySenderInviteRespondedAsync(int senderId, string inviteeName, string boardName, bool isAccepted)
    {
        var message = isAccepted
            ? $"{inviteeName} joined {boardName}!"
            : $"{inviteeName} declined the invitation to {boardName}.";

        var payload = new InvitationRespondedEvent(message, isAccepted);

        await _hubContext.Clients.User(senderId.ToString()).InviteResponded(payload);
    }

    public async Task NotifyUserAssignedToCardAsync(int userId, int cardId, string cardTitle)
    {
        await _hubContext.Clients.User(userId.ToString()).CardAssigned(cardId, cardTitle);
    }
}