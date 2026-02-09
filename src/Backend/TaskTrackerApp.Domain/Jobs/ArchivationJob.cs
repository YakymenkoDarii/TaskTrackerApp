namespace TaskTrackerApp.Domain.Jobs;

public class ArchivationJob
{
    [JsonProperty(PropertyName = "id")]
    public string id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty(PropertyName = "BoardId")]
    public int BoardId { get; set; }

    public string Status { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? BlobUrl { get; set; }

    public string? ErrorMessage { get; set; }
}