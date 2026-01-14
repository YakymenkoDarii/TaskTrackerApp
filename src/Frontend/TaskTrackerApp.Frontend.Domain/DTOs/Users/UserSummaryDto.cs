namespace TaskTrackerApp.Frontend.Domain.DTOs.Users;

public class UserSummaryDto
{
    public int Id { get; set; }

    public string DisplayName { get; set; }

    public string Email { get; set; }

    public string Tag { get; set; }

    public string AvatarUrl { get; set; }
}