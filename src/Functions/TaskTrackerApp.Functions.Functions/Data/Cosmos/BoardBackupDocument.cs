using Newtonsoft.Json;

namespace TaskTrackerApp.Functions.Functions.Data.Cosmos;

public class BoardBackupDocument
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("BoardId")]
    public int BoardId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string BlobUrl { get; set; } = string.Empty;

    public DateTime ArchivedAt { get; set; } = DateTime.UtcNow;

    public string ArchivedBy { get; set; } = "System";
}