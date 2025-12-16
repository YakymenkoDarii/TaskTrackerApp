namespace TaskTrackerApp.Domain.DTOs.Auth;

public class LoginRequest
{
    public string? Email { get; set; }

    public string? Tag { get; set; }

    public string Password { get; set; }
}