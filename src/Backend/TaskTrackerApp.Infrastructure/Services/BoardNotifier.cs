using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Interfaces.Hubs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Events.Column;
using TaskTrackerApp.Infrastructure.Hubs;

namespace TaskTrackerApp.Infrastructure.Services;

public class BoardNotifier : IBoardNotifier
{
    private readonly IHubContext<BoardHub, IBoardClient> _hubContext;

    public BoardNotifier(IHubContext<BoardHub, IBoardClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyColumnCreatedAsync(ColumnCreatedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").ColumnCreated(e);
    }

    public async Task NotifyColumnMovedAsync(ColumnMovedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").ColumnMoved(e);
    }

    public async Task NotifyColumnDeletedAsync(ColumnDeletedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").ColumnDeleted(e);
    }

    public async Task NotifyCardCreatedAsync(CardCreatedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").CardCreated(e);
    }

    public async Task NotifyCardMovedAsync(CardMovedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").CardMoved(e);
    }

    public async Task NotifyCardUpdatedAsync(CardUpdatedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").CardUpdated(e);
    }

    public async Task NotifyCardDeletedAsync(CardDeletedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").CardDeleted(e);
    }
}