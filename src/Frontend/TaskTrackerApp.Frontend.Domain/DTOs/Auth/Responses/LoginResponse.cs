using TaskTrackerApp.Frontend.Domain.Enums;

namespace TaskTrackerApp.Frontend.Domain.DTOs.Auth.Responses;

public class LoginResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public string Tag { get; set; } = default!;
    public Role Role { get; set; } = Role.User;
}