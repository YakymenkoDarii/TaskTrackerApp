using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTrackerApp.Application.Features.Cards.Commands.CreateCard;
using TaskTrackerApp.Application.Features.Cards.Commands.DeleteCard;
using TaskTrackerApp.Application.Features.Cards.Commands.UpdateCards;
using TaskTrackerApp.Application.Features.Cards.Queries.GetCardsByColumnId;
using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdString, out int userId))
        {
            return Unauthorized();
        }

        var command = new CreateCardCommand
        {
            Title = cardDto.Title,
            Description = cardDto.Description,
            DueDate = cardDto.DueDate,
            ColumnId = cardDto.ColumnId,
            BoardId = cardDto.BoardId,
            AssigneeId = cardDto.AssigneeId,
            CreatedById = userId,
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
            IsCompleted = cardDto.IsCompleted,
            Position = cardDto.Position
        };

        var updatedCard = await _mediator.Send(command);

        return Ok(updatedCard);
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

    [HttpGet("{columnId}")]
    public async Task<IActionResult> GetCardsByColumnId(int columnId)
    {
        var query = new GetCardsByColumnIdQuery(columnId);

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}