using StackExchange.Redis;
using System.Text.Json;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Infrastructure.Services;

public class ChatHistoryService : IChatHistoryService
{
    private readonly IConnectionMultiplexer _redis;
    private const int HistoryExpirationHours = 24;

    public ChatHistoryService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task AddMessageAsync(string sessionId, string role, string content)
    {
        var db = _redis.GetDatabase();
        var key = $"chat:{sessionId}";

        var message = new ChatMessage { Role = role, Content = content };
        var json = JsonSerializer.Serialize(message);

        await db.ListRightPushAsync(key, json);
        await db.KeyExpireAsync(key, TimeSpan.FromHours(HistoryExpirationHours));
    }

    public async Task<IEnumerable<ChatMessage>> GetHistoryAsync(string sessionId)
    {
        var db = _redis.GetDatabase();
        var key = $"chat:{sessionId}";

        var redisValues = await db.ListRangeAsync(key);

        return redisValues
            .Select(v => JsonSerializer.Deserialize<ChatMessage>(v.ToString()))
            .ToList();
    }
}