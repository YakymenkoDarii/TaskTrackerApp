using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Hubs;

namespace TaskTrackerApp.Infrastructure.Hubs;

[Authorize]
public class InvitationHub : Hub<IInvitationClient>
{
}