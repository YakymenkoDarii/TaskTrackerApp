using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Cards.Commands.CreateCard;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}
