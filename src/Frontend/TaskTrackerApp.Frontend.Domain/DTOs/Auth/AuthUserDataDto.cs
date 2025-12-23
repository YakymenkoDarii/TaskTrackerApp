namespace TaskTrackerApp.Frontend.Domain.DTOs.Auth;

public class AuthUserDataDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string Tag { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}