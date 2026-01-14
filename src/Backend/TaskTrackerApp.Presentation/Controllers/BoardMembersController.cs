using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.BoardMembers.Commands.DeleteMember;
using TaskTrackerApp.Application.Features.BoardMembers.Commands.UpdateMemberRole;
using TaskTrackerApp.Application.Features.BoardMembers.Queries;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoardMembersController : ControllerBase
{
    private IMediator _mediator;

    public BoardMembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{boardId}")]
    public async Task<IActionResult> GetMembersAsync(int boardId)
    {
        var query = new GetMembersQuery
        {
            BoardId = boardId
        };

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPut("{boardId}/{userId}")]
    public async Task<IActionResult> UpdateMemberRoleAsync(int boardId, int userId, BoardRole newRole)
    {
        var command = new UpdateMemberRoleCommand
        {
            BoardId = boardId,
            MemberId = userId,
            Role = newRole
        };

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{boardId}/{userId}")]
    public async Task<IActionResult> DeleteMemberAsync(int boardId, int userId)
    {
        var command = new DeleteMemberCommand
        {
            BoardId = boardId,
            MemberId = userId
        };

        var result = await _mediator.Send(command);

        return Ok(result);
    }
}