using Newtonsoft.Json;

namespace TaskTrackerApp.Functions.Functions.Data.Cosmos;

public class ArchivationJob
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("BoardId")]
    public int BoardId { get; set; }

    public string Status { get; set; } = "Started";

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public string? FailureReason { get; set; }
}