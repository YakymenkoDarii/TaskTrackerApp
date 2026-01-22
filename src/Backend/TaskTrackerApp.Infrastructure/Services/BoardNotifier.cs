using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Interfaces.Hubs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Events.BoardMember;
using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Events.Column;
using TaskTrackerApp.Domain.Events.Invitations;
using TaskTrackerApp.Domain.Events.Labels;
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
        var groupName = $"Board_{e.BoardId}";

        // --- ADD THIS LOG ---
        Console.WriteLine($"[SERVER NOTIFIER] Sending ColumnCreated to group: {groupName}");
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

    public async Task NotifyMemberAddedAsync(BoardMemberAddedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").MemberAdded(e);
    }

    public async Task NotifyMemberRemovedAsync(BoardMemberRemovedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").MemberRemoved(e);
    }

    public async Task NotifyMemberRoleUpdatedAsync(BoardMemberRoleUpdatedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").MemberRoleUpdated(e);
    }

    public async Task NotifyInvitationAddedAsync(BoardInvitationAddedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").InvitationAdded(e);
    }

    public async Task NotifyInvitationRevokedAsync(BoardInvitationRevokedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").InvitationRevoked(e);
    }

    public async Task NotifyLabelCreatedAsync(LabelCreatedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").LabelCreated(e);
    }

    public async Task NotifyLabelUpdatedAsync(LabelUpdatedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").LabelUpdated(e);
    }

    public async Task NotifyLabelDeletedAsync(LabelDeletedEvent e)
    {
        await _hubContext.Clients.Group($"Board_{e.BoardId}").LabelDeleted(e);
    }
}