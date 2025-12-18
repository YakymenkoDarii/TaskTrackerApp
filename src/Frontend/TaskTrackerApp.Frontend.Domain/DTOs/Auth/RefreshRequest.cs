namespace TaskTrackerApp.Frontend.Domain.DTOs.Auth;

public sealed class RefreshRequest
{
    public string RefreshToken { get; set; }

    public string AccessToken { get; set; }
}