using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Interfaces.Hubs;

namespace TaskTrackerApp.Infrastructure.Hubs;

[Authorize]
public class CardHub : Hub<ICardClient>
{
    public async Task JoinCardGroup(int cardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Card_{cardId}");
    }

    public async Task LeaveCardGroup(int cardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Card_{cardId}");
    }
}