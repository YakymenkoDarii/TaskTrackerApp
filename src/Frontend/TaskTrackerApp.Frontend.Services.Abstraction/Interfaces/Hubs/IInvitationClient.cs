using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Events.Invitations;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Hubs;

public interface IInvitationClient
{
    Task ReceiveInvite(InvitationReceivedEvent notification);

    Task RevokeInvite(int invitationId);

    Task InviteResponded(InvitationRespondedEvent notification);
}