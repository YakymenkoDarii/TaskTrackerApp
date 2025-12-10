namespace TaskTrackerApp.Domain.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Tag { get; set; }
    public string DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
