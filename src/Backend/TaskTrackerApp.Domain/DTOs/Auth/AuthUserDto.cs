using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.DTOs.Auth;

public class AuthUserDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}