namespace TaskTrackerApp.Frontend.Domain.DTOs.Auth.Requests;

public sealed class LoginRequest
{
    public string? Email { get; set; }

    public string? Tag { get; set; }

    public string Password { get; set; }
}