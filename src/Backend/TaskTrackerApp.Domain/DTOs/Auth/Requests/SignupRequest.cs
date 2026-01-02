namespace TaskTrackerApp.Domain.DTOs.Auth;

public class SignupRequest
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string Tag { get; set; }

    public string DisplayName { get; set; }
}