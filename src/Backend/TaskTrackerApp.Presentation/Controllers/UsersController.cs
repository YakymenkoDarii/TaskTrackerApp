using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Users.Commands.CreateUsers;
using TaskTrackerApp.Application.Features.Users.Commands.DeleteUsers;
using TaskTrackerApp.Application.Features.Users.Commands.UpdateUsers;
using TaskTrackerApp.Domain.DTOs.User;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateUserDto userDto)
    {
        var command = new UpdateUserCommand
        {
            Id = id,
            Tag = userDto.Tag,
            PasswordHash = userDto.PasswordHash,
            DisplayName = userDto.DisplayName,
            AvatarUrl = userDto.AvatarUrl
        };

        await _mediator.Send(command);

        return Ok(command);
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
}