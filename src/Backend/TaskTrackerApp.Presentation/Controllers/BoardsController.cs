using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskTrackerApp.Application.Features.Boards.Queries.GetAllBoards;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private IMediator _mediator;

    public BoardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("boards")]
    public async Task<IActionResult> GetAllAsync(int userId)
    {
        GetAllBoardsQuery query = new GetAllBoardsQuery(userId);

        var result = await _mediator.Send(query);

        return result is null
            ? NotFound()
            : Ok(result);
    }
}