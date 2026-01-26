using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Interfaces.Hubs;

namespace TaskTrackerApp.Infrastructure.Hubs;

[Authorize]
public class InvitationHub : Hub<IInvitationClient>
{
}