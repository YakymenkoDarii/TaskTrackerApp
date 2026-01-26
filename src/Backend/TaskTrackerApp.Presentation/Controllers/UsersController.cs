using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Users.Commands.ChangePassword;
using TaskTrackerApp.Application.Features.Users.Commands.CreateUsers;
using TaskTrackerApp.Application.Features.Users.Commands.DeleteUsers;
using TaskTrackerApp.Application.Features.Users.Commands.UpdateUserAvatar;
using TaskTrackerApp.Application.Features.Users.Commands.UpdateUsers;
using TaskTrackerApp.Application.Features.Users.Queries.GetMyProfile;
using TaskTrackerApp.Application.Features.Users.Queries.SearchUsersQuery;
using TaskTrackerApp.Domain.DTOs.User;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto userDto)
    {
        var command = new CreateUserCommand
        {
            Email = userDto.Email,
            PasswordHash = userDto.PasswordHash,
            Tag = userDto.Tag,
            DisplayName = userDto.DisplayName
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserDto userDto)
    {
        var command = new UpdateUserCommand
        {
            Tag = userDto.Tag,
            DisplayName = userDto.DisplayName,
        };

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var command = new DeleteUserCommand
        {
            Id = id
        };

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] string term, [FromQuery] int? excludeBoardId)
    {
        var query = new SearchUsersQuery
        {
            SearchTerm = term,
            ExcludeBoard = excludeBoardId
        };

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPut("update-avatar")]
    public async Task<IActionResult> UpdateAvatarAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var command = new UpdateUserAvatarCommand
        {
            FileContent = stream,
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        var result = await _mediator.Send(command);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var command = new ChangePasswordCommand
        {
            OldPassword = request.OldPassword,
            NewPassword = request.NewPassword,
            NewPasswordConfirm = request.ConfirmPassword,
        };

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentProfile()
    {
        var query = new GetMyProfileQuery();

        var result = await _mediator.Send(query);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}