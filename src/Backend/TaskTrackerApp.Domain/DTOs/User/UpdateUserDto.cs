namespace TaskTrackerApp.Domain.DTOs.User;

public class UpdateUserDto
{
    public int Id { get; set; }

    public string Tag { get; set; }

    public string PasswordHash { get; set; }

    public string DisplayName { get; set; }

    public string AvatarUrl { get; set; }
}