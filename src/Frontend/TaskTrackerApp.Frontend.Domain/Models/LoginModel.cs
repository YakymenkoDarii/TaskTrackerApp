using System.ComponentModel.DataAnnotations;

namespace TaskTrackerApp.Frontend.Domain.Models;

public sealed class LoginModel
{
    [Required(ErrorMessage = "Email or tag is required")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}