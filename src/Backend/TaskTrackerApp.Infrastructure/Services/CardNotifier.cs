using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Interfaces.Hubs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Events.Comment;
using TaskTrackerApp.Infrastructure.Hubs;

namespace TaskTrackerApp.Infrastructure.Services;

public class CardNotifier : ICardNotifier
{
    private readonly IHubContext<CardHub, ICardClient> _hubContext;

    public CardNotifier(IHubContext<CardHub, ICardClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyCommentAddedAsync(CommentAddedEvent e)
    {
        await _hubContext.Clients.Group($"Card_{e.CardId}").CommentAdded(e);
    }

    public async Task NotifyCommentUpdatedAsync(CommentUpdatedEvent e)
    {
        await _hubContext.Clients.Group($"Card_{e.CardId}").CommentUpdated(e);
    }

    public async Task NotifyCommentDeletedAsync(int commentId, int cardId)
    {
        await _hubContext.Clients.Group($"Card_{cardId}").CommentDeleted(commentId);
    }

    public async Task NotifyLabelAddedAsync(int cardId, int labelId)
    {
        await _hubContext.Clients.Group($"Card_{cardId}").LabelAdded(cardId, labelId);
    }

    public async Task NotifyLabelRemovedAsync(int cardId, int labelId)
    {
        await _hubContext.Clients.Group($"Card_{cardId}").LabelRemoved(cardId, labelId);
    }
}