using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Cards.Commands.CreateCard;
using TaskTrackerApp.Application.Features.Cards.Commands.DeleteCard;
using TaskTrackerApp.Application.Features.Cards.Commands.UpdateCards;
using TaskTrackerApp.Domain.DTOs.Card;

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
    public async Task<IActionResult> CreateAsync([FromBody] CreateCardDto cardDto)
    {
        var command = new CreateCardCommand
        {
            Title = cardDto.Title,
            Description = cardDto.Description,
            DueDate = cardDto.DueDate,
            ColumnId = cardDto.ColumnId,
            BoardId = cardDto.BoardId,
            AssigneeId = cardDto.AssigneeId,
            CreatedById = cardDto.CreatedById,
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateCardDto cardDto)
    {
        var command = new UpdateCardCommand
        {
            Id = id,
            Title = cardDto.Title,
            Description = cardDto.Description,
            DueDate = cardDto.DueDate,
            ColumnId = cardDto.ColumnId,
            BoardId = cardDto.BoardId,
            AssigneeId = cardDto.AssigneeId,
            UpdatedById = cardDto.UpdatedById,
        };

        await _mediator.Send(command);

        return Ok(command);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var command = new DeleteCardCommand
        {
            Id = id
        };

        await _mediator.Send(command);

        return NoContent();
    }
}