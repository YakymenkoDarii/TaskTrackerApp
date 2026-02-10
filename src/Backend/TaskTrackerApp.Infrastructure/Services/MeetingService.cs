using StackExchange.Redis;
using System.Text.Json;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.DTOs.Meeting;

namespace TaskTrackerApp.Infrastructure.Services;

public class MeetingService : IMeetingService
{
    private readonly IDatabase _db;
    private const int RedisDbIndex = 1;
    private readonly TimeSpan _expiry = TimeSpan.FromHours(24);

    public MeetingService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase(RedisDbIndex);
    }

    public MeetingDto? GetMeeting(int boardId)
    {
        var key = $"meeting:{boardId}";
        var json = _db.StringGet(key);

        if (json.IsNullOrEmpty) return null;

        return JsonSerializer.Deserialize<MeetingDto>(json);
    }

    public MeetingDto StartOrJoinMeeting(int boardId, string peerId)
    {
        var key = $"meeting:{boardId}";
        var json = _db.StringGet(key);
        MeetingDto meeting;

        if (json.IsNullOrEmpty)
        {
            meeting = new MeetingDto
            {
                BoardId = boardId,
                StartTime = DateTime.UtcNow,
                ParticipantPeerIds = new List<string> { peerId }
            };
        }
        else
        {
            meeting = JsonSerializer.Deserialize<MeetingDto>(json)!;
            if (!meeting.ParticipantPeerIds.Contains(peerId))
            {
                meeting.ParticipantPeerIds.Add(peerId);
            }
        }
        _db.StringSet(key, JsonSerializer.Serialize(meeting), _expiry);

        return meeting;
    }

    public void LeaveMeeting(int boardId, string peerId)
    {
        var key = $"meeting:{boardId}";
        var json = _db.StringGet(key);

        if (!json.IsNullOrEmpty)
        {
            var meeting = JsonSerializer.Deserialize<MeetingDto>(json)!;

            if (meeting.ParticipantPeerIds.Contains(peerId))
            {
                meeting.ParticipantPeerIds.Remove(peerId);

                if (meeting.ParticipantPeerIds.Count == 0)
                {
                    _db.KeyDelete(key);
                }
                else
                {
                    _db.StringSet(key, JsonSerializer.Serialize(meeting), _expiry);
                }
            }
        }
    }
}