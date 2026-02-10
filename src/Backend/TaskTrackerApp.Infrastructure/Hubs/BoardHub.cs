using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TaskTrackerApp.Application.Interfaces.Hubs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.DTOs.Meeting;

namespace TaskTrackerApp.Infrastructure.Hubs;

[Authorize]
public class BoardHub : Hub<IBoardClient>
{
    private readonly IMeetingService _meetingService;

    public BoardHub(IMeetingService meetingService)
    {
        _meetingService = meetingService;
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

    public async Task JoinMeeting(int boardId, string peerId)
    {
        // 1. SAVE CONTEXT: Remember that this ConnectionId = this PeerId + BoardId
        Context.Items["MeetingBoardId"] = boardId;
        Context.Items["MeetingPeerId"] = peerId;

        // 2. Standard Logic
        var meeting = _meetingService.StartOrJoinMeeting(boardId, peerId);
        var others = meeting.ParticipantPeerIds.Where(p => p != peerId).ToList();

        await Clients.Caller.JoinMeetingResponse(others);
        await Clients.Group($"Board_{boardId}").MeetingStateUpdated(meeting);
        await Clients.Group($"Board_{boardId}").UserJoinedMeeting(peerId);
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

    private async Task HandleLeave(int boardId, string peerId)
    {
        _meetingService.LeaveMeeting(boardId, peerId);
        var meeting = _meetingService.GetMeeting(boardId);

        await Clients.Group($"Board_{boardId}").UserLeftMeeting(peerId);
        await Clients.Group($"Board_{boardId}").MeetingStateUpdated(meeting);
    }

    public MeetingDto? GetActiveMeeting(int boardId)
    {
        return _meetingService.GetMeeting(boardId);
    }
}