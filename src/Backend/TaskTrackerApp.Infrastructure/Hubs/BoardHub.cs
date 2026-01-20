using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Interfaces.Hubs;

namespace TaskTrackerApp.Infrastructure.Hubs;

[Authorize]
public class BoardHub : Hub<IBoardClient>
{
    public async Task JoinBoardGroup(int boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Board_{boardId}");
    }

    public async Task LeaveBoardGroup(int boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Board_{boardId}");
    }
}