namespace TaskTrackerApp.Domain.DTOs.User;
public class CreateUserDto
{
    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string Tag { get; set; }

    public string DisplayName { get; set; }
}
