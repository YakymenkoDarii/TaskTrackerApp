namespace TaskTrackerApp.Domain.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}