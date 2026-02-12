using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Features.Meeting.Queries.GetMeetingParticipantWithPeerId;
using TaskTrackerApp.Application.Interfaces.Hubs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.DTOs.Meeting;

namespace TaskTrackerApp.Infrastructure.Hubs;

[Authorize]
public class BoardHub : Hub<IBoardClient>
{
    private readonly IMeetingService _meetingService;
    private readonly IMediator _mediator;

    public BoardHub(IMeetingService meetingService, IMediator mediator)
    {
        _meetingService = meetingService;
        _mediator = mediator;
    }

    public async Task JoinBoardGroup(int boardId)
    {
        var groupName = $"Board_{boardId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        Console.WriteLine($"[Hub] Connection {Context.ConnectionId} joined group {groupName}");
    }

    public async Task LeaveBoardGroup(int boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Board_{boardId}");
    }

    public async Task JoinMeeting(int boardId, string peerId, bool isMuted, bool isVideoOff)
    {
        var query = new GetMeetingParticipantWithPeerIdQuery
        {
            PeerId = peerId,
            IsMuted = isMuted,
            IsVideoOff = isVideoOff
        };

        var result = await _mediator.Send(query);
        var participant = result.Value;

        var meeting = _meetingService.StartOrJoinMeeting(boardId, participant);

        Context.Items["MeetingBoardId"] = boardId;
        Context.Items["MeetingPeerId"] = peerId;

        var others = meeting.Participants
                                        .Where(p => p.PeerId != peerId)
                                        .ToList();

        await Clients.Caller.JoinMeetingResponse(others);

        await Clients.Group($"Board_{boardId}").UserJoinedMeeting(participant);
        await Clients.Group($"Board_{boardId}").MeetingStateUpdated(meeting);
    }

    public async Task LeaveMeeting(int boardId, string peerId)
    {
        await HandleLeave(boardId, peerId);

        Context.Items.Remove("MeetingBoardId");
        Context.Items.Remove("MeetingPeerId");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue("MeetingBoardId", out var boardIdObj) &&
            Context.Items.TryGetValue("MeetingPeerId", out var peerIdObj))
        {
            if (boardIdObj is int boardId && peerIdObj is string peerId)
            {
                await HandleLeave(boardId, peerId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task UpdateMediaState(int boardId, string peerId, bool isMuted, bool isVideoOff)
    {
        var meeting = _meetingService.GetMeeting(boardId);
        if (meeting != null)
        {
            var participant = meeting.Participants.FirstOrDefault(p => p.PeerId == peerId);
            if (participant != null)
            {
                participant.IsMuted = isMuted;
                participant.IsVideoOff = isVideoOff;
                _meetingService.StartOrJoinMeeting(boardId, participant);
            }
        }

        await Clients.Group($"Board_{boardId}").ParticipantStateUpdated(peerId, isMuted, isVideoOff);
    }

    private async Task HandleLeave(int boardId, string peerId)
    {
        _meetingService.LeaveMeeting(boardId, peerId);
        var meeting = _meetingService.GetMeeting(boardId);

        await Clients.Group($"Board_{boardId}").UserLeftMeeting(peerId);
        await Clients.Group($"Board_{boardId}").MeetingStateUpdated(meeting);
    }

    public async Task StopScreenShare(int boardId, string peerId)
    {
        await Clients.Group($"Board_{boardId}").UserStoppedScreenShare(peerId);
    }

    public MeetingDto? GetActiveMeeting(int boardId)
    {
        return _meetingService.GetMeeting(boardId);
    }
}