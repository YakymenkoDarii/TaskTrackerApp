using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTrackerApp.Application.Features.BoardInvitations.Commands.RespondToInvitation;
using TaskTrackerApp.Application.Features.BoardInvitations.Commands.RevokeBoardInvitation;
using TaskTrackerApp.Application.Features.BoardInvitations.Commands.SendBoardInvitation;
using TaskTrackerApp.Application.Features.BoardInvitations.Queries.GetMyPendingInvitations;
using TaskTrackerApp.Application.Features.BoardInvitations.Queries.GetPendingInvites;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoardInvitationsController : ControllerBase
{
    private IMediator _mediator;

    public BoardInvitationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{boardId}")]
    public async Task<IActionResult> GetPendingInvitesAsync(int boardId)
    {
        var query = new GetPendingInvitesQuery
        {
            BoardId = boardId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("invite")]
    public async Task<IActionResult> SendInviteAsync(SendBoardInvitationRequestDto request)
    {
        var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? throw new UnauthorizedAccessException());

        var comamnd = new SendBoardInvitationCommand
        {
            BoardId = request.BoardId,
            InviteeEmail = request.InviteeEmail,
            Role = request.Role,
            SenderId = senderId,
        };

        var result = await _mediator.Send(comamnd);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{invitationId}")]
    public async Task<IActionResult> RevokeInviteAsync(int invitationId)
    {
        var comamnd = new RevokeBoardInvitationCommand
        {
            InvitationId = invitationId
        };

        var result = await _mediator.Send(comamnd);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("my-pending")]
    public async Task<IActionResult> GetMyPendingInvitations()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var query = new GetMyPendingInvitationsQuery
        {
            UserId = userId
        };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost("respond")]
    public async Task<IActionResult> RespondToInvite([FromBody] RespondToInvitationRequestDto request)
    {
        var command = new RespondToInvitationCommand
        {
            InvitationId = request.InvitationId,
            IsAccepted = request.IsAccepted
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}