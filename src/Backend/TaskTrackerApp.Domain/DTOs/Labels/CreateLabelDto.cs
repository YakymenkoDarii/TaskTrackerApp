namespace TaskTrackerApp.Domain.DTOs.Labels;

public class CreateLabelDto
{
    public string Name { get; set; }

    public string Color { get; set; }

    public int BoardId { get; set; }
}