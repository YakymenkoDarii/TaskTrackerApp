namespace TaskTrackerApp.Domain.DTOs.Auth;

public class RefreshRequest
{
    public string RefreshToken { get; set; }

    public string AccessToken { get; set; }
}