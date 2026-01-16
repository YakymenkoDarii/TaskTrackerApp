using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.CardComments.Commands.CreateCardCommentCommand;
using TaskTrackerApp.Application.Features.CardComments.Commands.DeleteCardCommentCommand;
using TaskTrackerApp.Application.Features.CardComments.Commands.UpdateCardCommen;
using TaskTrackerApp.Application.Features.CardComments.Queries;
using TaskTrackerApp.Domain.DTOs.CardComment;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardCommentsController : ControllerBase
{
    private IMediator _mediator;

    public CardCommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{cardId}")]
    public async Task<IActionResult> GetCommentsByCardIdAsync(int cardId)
    {
        var query = new GetCommentsByCardIdQuery
        {
            CardId = cardId
        };

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCommentAsync(CreateCardCommentDto createDto)
    {
        var command = new CreateCardCommentCommand
        {
            CardId = createDto.CardId,
            Text = createDto.Text,
            CreatedById = createDto.CreatedById,
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCommentAsync(int id)
    {
        var command = new DeleteCardCommentCommand
        {
            Id = id
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCommentAsync(UpdateCardCommentDto updateDto)
    {
        var command = new UpdateCardCommentCommand
        {
            Id = updateDto.Id,
            Text = updateDto.Text,
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}