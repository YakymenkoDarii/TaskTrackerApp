using TaskTrackerApp.Domain.Events.Comment;

namespace TaskTrackerApp.Application.Interfaces.Services;

public interface ICardNotifier
{
    Task NotifyCommentAddedAsync(CommentAddedEvent e);

    Task NotifyCommentUpdatedAsync(CommentUpdatedEvent e);

    Task NotifyCommentDeletedAsync(int commentId, int cardId);

    Task NotifyLabelAddedAsync(int cardId, int labelId);
}