using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Events.Column;

namespace TaskTrackerApp.Application.Interfaces.Hubs;

public interface IBoardClient
{
    Task ColumnCreated(ColumnCreatedEvent e);

    Task ColumnMoved(ColumnMovedEvent e);

    Task ColumnDeleted(ColumnDeletedEvent e);

    Task CardCreated(CardCreatedEvent e);

    Task CardMoved(CardMovedEvent e);

    Task CardUpdated(CardUpdatedEvent e);

    Task CardDeleted(CardDeletedEvent e);
}