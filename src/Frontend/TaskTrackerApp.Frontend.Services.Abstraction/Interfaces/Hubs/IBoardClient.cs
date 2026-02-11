using TaskTrackerApp.Frontend.Domain.DTOs.Meeting;
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

    //Meeting
    Task MeetingStateUpdated(MeetingDto? meeting);

    Task JoinMeetingResponse(List<string> participantPeerIds);

    Task UserJoinedMeeting(string peerId);

    Task UserLeftMeeting(string peerId);

    Task ParticipantStateUpdated(string peerId, bool isMuted, bool isVideoOff);

    Task UserStoppedScreenShare(string peerId);
}