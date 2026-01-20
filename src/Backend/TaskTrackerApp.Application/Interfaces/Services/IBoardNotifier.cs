using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Events.Column;

namespace TaskTrackerApp.Application.Interfaces.Services;

public interface IBoardNotifier
{
    Task NotifyColumnCreatedAsync(ColumnCreatedEvent e);

    Task NotifyColumnMovedAsync(ColumnMovedEvent e);

    Task NotifyColumnDeletedAsync(ColumnDeletedEvent e);

    Task NotifyCardCreatedAsync(CardCreatedEvent e);

    Task NotifyCardMovedAsync(CardMovedEvent e);

    Task NotifyCardUpdatedAsync(CardUpdatedEvent e);

    Task NotifyCardDeletedAsync(CardDeletedEvent e);
}