using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Auth.Commands.LoginCommand;
using TaskTrackerApp.Application.Features.Auth.Commands.RefreshTokenCommand;
using TaskTrackerApp.Application.Features.Auth.Commands.SignupCommand;
using TaskTrackerApp.Domain.DTOs.Auth;
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
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password,
            Tag = request.Tag
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error.Code switch
        {
            var code when code == LoginError.UserNotFound.Code
                => Unauthorized(result.Error),

            var code when code == LoginError.InvalidPassword.Code
                => Unauthorized(result.Error),

            _ => BadRequest(result.Error)
        };
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignupAsync([FromBody] SignupRequest request)
    {
        var command = new SignupCommand
        {
            Email = request.Email,
            Password = request.Password,
            DisplayName = request.DisplayName,
            Tag = request.Tag
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error.Code switch
        {
            var code when code == SignupError.EmailInUse.Code
                => Unauthorized(result.Error),

            var code when code == SignupError.TagInUse.Code
                => Unauthorized(result.Error),

            _ => BadRequest(result.Error)
        };
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshRequest request)
    {
        var command = new RefreshTokenCommand
        {
            AccessToken = request.AccessToken,
            RefreshToken = request.RefreshToken
        };

        var result = await _mediator.Send(command);

        return result is null
            ? BadRequest()
            : Ok(result);
    }
}