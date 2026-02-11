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

    public MeetingDto StartOrJoinMeeting(int boardId, MeetingParticipant participant)
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
                Participants = new List<MeetingParticipant> { participant }
            };
        }
        else
        {
            meeting = JsonSerializer.Deserialize<MeetingDto>(json)!;

            var existing = meeting.Participants.FirstOrDefault(p => p.PeerId == participant.PeerId);
            if (existing == null)
            {
                meeting.Participants.Add(participant);
            }
            else
            {
                existing.DisplayName = participant.DisplayName;
                existing.AvatarUrl = participant.AvatarUrl;
                existing.IsMuted = participant.IsMuted;
                existing.IsVideoOff = participant.IsVideoOff;
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

            var participantToRemove = meeting.Participants.FirstOrDefault(p => p.PeerId == peerId);

            if (participantToRemove != null)
            {
                meeting.Participants.Remove(participantToRemove);

                if (meeting.Participants.Count == 0)
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