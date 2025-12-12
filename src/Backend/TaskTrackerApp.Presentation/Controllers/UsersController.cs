using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;
using TaskTrackerApp.Application.Features.Users.Commands.CreateUsers;
using TaskTrackerApp.Domain.DTOs.Board;
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
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto boardDto)
    {
        var command = new CreateUserCommand
        {
            Email = boardDto.Email,
            PasswordHash = boardDto.PasswordHash,
            Tag = boardDto.Tag,
            DisplayName = boardDto.DisplayName
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }


}
