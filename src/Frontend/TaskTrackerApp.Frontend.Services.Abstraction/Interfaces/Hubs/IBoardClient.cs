using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.Events.BoardMember;
using TaskTrackerApp.Frontend.Domain.Events.Card;
using TaskTrackerApp.Frontend.Domain.Events.Column;
using TaskTrackerApp.Frontend.Domain.Events.Invitations;
using TaskTrackerApp.Frontend.Domain.Events.Labels;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Hubs;

public interface IBoardClient
{
    Task ColumnCreated(ColumnCreatedEvent e);

    Task ColumnMoved(ColumnMovedEvent e);

    Task ColumnDeleted(ColumnDeletedEvent e);

    Task CardCreated(CardCreatedEvent e);

    Task CardMoved(CardMovedEvent e);

    Task CardUpdated(CardUpdatedEvent e);

    Task CardDeleted(CardDeletedEvent e);

    Task MemberAdded(BoardMemberAddedEvent e);

    Task MemberRemoved(BoardMemberRemovedEvent e);

    Task MemberRoleUpdated(BoardMemberRoleUpdatedEvent e);

    Task InvitationAdded(BoardInvitationAddedEvent e);

    Task InvitationRevoked(BoardInvitationRevokedEvent e);

    Task LabelCreated(LabelCreatedEvent e);

    Task LabelUpdated(LabelUpdatedEvent e);

    Task LabelDeleted(LabelDeletedEvent e);
}