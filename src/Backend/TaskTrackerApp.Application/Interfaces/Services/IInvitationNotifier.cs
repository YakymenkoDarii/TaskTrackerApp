using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Interfaces.Services;

public interface IInvitationNotifier
{
    Task NotifyUserReceivedInviteAsync(int userId, int invitationId, int boardId, string senderName, string boardName);

    Task NotifyUserInviteRevokedAsync(int userId, int invitationId);

    Task NotifySenderInviteRespondedAsync(int senderId, string inviteeName, string boardName, bool isAccepted);
}