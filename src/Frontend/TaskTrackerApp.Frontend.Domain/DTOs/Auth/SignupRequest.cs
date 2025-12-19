namespace TaskTrackerApp.Frontend.Domain.DTOs.Auth;

public sealed class SignupRequest
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string Tag { get; set; }

    public string DisplayName { get; set; }
}