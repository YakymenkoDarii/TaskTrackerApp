namespace TaskTrackerApp.Domain.DTOs.Auth.Responses;

public class AuthResponse
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}