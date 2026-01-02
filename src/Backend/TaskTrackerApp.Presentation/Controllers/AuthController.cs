using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Auth.Commands.LoginCommand;
using TaskTrackerApp.Application.Features.Auth.Commands.RefreshTokenCommand;
using TaskTrackerApp.Application.Features.Auth.Commands.SignupCommand;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.DTOs.Auth.Requests;
using TaskTrackerApp.Domain.Errors;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

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

        if (!string.IsNullOrEmpty(result.Value.RefreshToken))
        {
            SetRefreshTokenCookie(
                result.Value.RefreshToken,
                result.Value.RefreshTokenExpiration
            );
        }

        result.Value.RefreshToken = null;
        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var refreshTokenFromCookie = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshTokenFromCookie))
            return Unauthorized(new Error("Auth.NoToken", "No refresh token found."));

        var result = await _mediator.Send(new RefreshTokenCommand
        {
            Tag = request.Tag,
            RefreshToken = refreshTokenFromCookie
        });

        if (!result.IsSuccess)
        {
            DeleteRefreshTokenCookie();
            return Unauthorized(result.Error);
        }

        if (!string.IsNullOrEmpty(result.Value.RefreshToken))
        {
            SetRefreshTokenCookie(
                result.Value.RefreshToken,
                result.Value.RefreshTokenExpiration
            );
        }

        result.Value.RefreshToken = null;
        return Ok(result.Value);
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
                var code when code == SignupError.EmailInUse.Code => Unauthorized(result.Error),
                var code when code == SignupError.TagInUse.Code => Unauthorized(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        DeleteRefreshTokenCookie();
        return Ok();
    }

    private void SetRefreshTokenCookie(string token, DateTime expiration)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expiration,

            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };

        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private void DeleteRefreshTokenCookie()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };

        Response.Cookies.Delete("refreshToken", cookieOptions);
    }
}