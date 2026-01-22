using TaskTrackerApp.Domain.Events.BoardMember;
using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Events.Column;
using TaskTrackerApp.Domain.Events.Invitations;
using TaskTrackerApp.Domain.Events.Labels;

namespace TaskTrackerApp.Application.Interfaces.Services;

public interface IBoardNotifier
{
    //Columns
    Task NotifyColumnCreatedAsync(ColumnCreatedEvent e);

    Task NotifyColumnMovedAsync(ColumnMovedEvent e);

    Task NotifyColumnDeletedAsync(ColumnDeletedEvent e);

    //Cards
    Task NotifyCardCreatedAsync(CardCreatedEvent e);

    Task NotifyCardMovedAsync(CardMovedEvent e);

    Task NotifyCardUpdatedAsync(CardUpdatedEvent e);

    Task NotifyCardDeletedAsync(CardDeletedEvent e);

    //Members
    Task NotifyMemberAddedAsync(BoardMemberAddedEvent e);

    Task NotifyMemberRemovedAsync(BoardMemberRemovedEvent e);

    Task NotifyMemberRoleUpdatedAsync(BoardMemberRoleUpdatedEvent e);

    //Invitations
    Task NotifyInvitationAddedAsync(BoardInvitationAddedEvent e);

    Task NotifyInvitationRevokedAsync(BoardInvitationRevokedEvent e);

    //Labels
    Task NotifyLabelCreatedAsync(LabelCreatedEvent e);

    Task NotifyLabelUpdatedAsync(LabelUpdatedEvent e);

    Task NotifyLabelDeletedAsync(LabelDeletedEvent e);
}