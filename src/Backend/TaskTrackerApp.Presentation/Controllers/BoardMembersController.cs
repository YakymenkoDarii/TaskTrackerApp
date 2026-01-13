using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.BoardMembers.Queries;

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
}