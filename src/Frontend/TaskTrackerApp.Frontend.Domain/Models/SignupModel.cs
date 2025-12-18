using System.ComponentModel.DataAnnotations;

namespace TaskTrackerApp.Frontend.Domain.Models;

public sealed class SignupModel
{
    [Required(ErrorMessage = "This field is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "This field is required")]
    public string Tag { get; set; } = string.Empty;

    [Required(ErrorMessage = "This field is required")]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}