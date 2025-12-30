using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTrackerApp.Application.Features.Auth.Commands.LoginCommand;
using TaskTrackerApp.Application.Features.Auth.Commands.RefreshTokenCommand;
using TaskTrackerApp.Application.Features.Auth.Commands.SignupCommand;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Errors;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand
        {
            Email = request.Email,
            Password = request.Password,
            Tag = request.Tag
        });

        if (!result.IsSuccess)
            return Unauthorized(result.Error);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, result.Value.Id.ToString()),
        new Claim(ClaimTypes.Name, result.Value.DisplayName),
        new Claim(ClaimTypes.Email, result.Value.Email),
        new Claim(ClaimTypes.Role, result.Value.Role.ToString()),
    };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        return Ok(result.Value.Id + "User Id");
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignupAsync([FromBody] SignupRequest request)
    {
        var result = await _mediator.Send(new SignupCommand
        {
            Email = request.Email,
            Password = request.Password,
            DisplayName = request.DisplayName,
            Tag = request.Tag
        });

        if (!result.IsSuccess)
        {
            return result.Error.Code switch
            {
                var code when code == SignupError.EmailInUse.Code
                    => Unauthorized(result.Error),

                var code when code == SignupError.TagInUse.Code
                    => Unauthorized(result.Error),

                _ => BadRequest(result.Error)
            };
        }

        var user = result.Value;
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.DisplayName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
    };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        return Ok(result.Value.Id + "User Id");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return Ok();
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        return Ok(new
        {
            Id = userId,
            Email = email,
            DisplayName = name
        });
    }
}