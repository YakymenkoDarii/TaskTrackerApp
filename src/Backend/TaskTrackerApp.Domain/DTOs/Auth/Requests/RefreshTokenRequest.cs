namespace TaskTrackerApp.Domain.DTOs.Auth.Requests;

public class RefreshTokenRequest
{
    public string? RefreshToken { get; set; } = default!;
    public string Tag { get; set; } = default!;
}