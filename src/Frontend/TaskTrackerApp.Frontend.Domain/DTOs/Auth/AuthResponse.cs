namespace TaskTrackerApp.Frontend.Domain.DTOs.Auth;

public sealed class AuthResponse
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}