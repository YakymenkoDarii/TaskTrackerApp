using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Labels.Command.AddLabelToCard;
using TaskTrackerApp.Application.Features.Labels.Command.CreateLabel;
using TaskTrackerApp.Application.Features.Labels.Command.DeleteLabel;
using TaskTrackerApp.Application.Features.Labels.Command.RemoveLabelFromCard;
using TaskTrackerApp.Application.Features.Labels.Command.UpdateLabel;
using TaskTrackerApp.Application.Features.Labels.Queries.GetLabelsByBoardId;
using TaskTrackerApp.Domain.DTOs.Labels;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabelsController : ControllerBase
{
    private IMediator _mediator;

    public LabelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateLabelAsync([FromBody] CreateLabelDto createDto)
    {
        var command = new CreateLabelCommand
        {
            CreateLabel = createDto
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateLabelAsync([FromBody] LabelDto updateDto)
    {
        var command = new UpdateLabelCommand
        {
            Dto = updateDto
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("{boardId}")]
    public async Task<IActionResult> GetLabelsByBoardIdAsync(int boardId)
    {
        var query = new GetLabelsByBoardIdQuery
        {
            BoardId = boardId
        };
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("{cardId}/{labelId}")]
    public async Task<IActionResult> AddLabelToCardAsync(int cardId, int labelId)
    {
        var command = new AddLabelToCardCommand
        {
            CardId = cardId,
            LabelId = labelId
        };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{cardId}/{labelId}")]
    public async Task<IActionResult> RemoveLabelFromCardAsync(int cardId, int labelId)
    {
        var command = new RemoveLabelFromCardCommand
        {
            CardId = cardId,
            LabelId = labelId
        };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("delete/{labelId}")]
    public async Task<IActionResult> DeleteLabelAsync(int labelId)
    {
        var command = new DeleteLabelCommand
        {
            LabelId = labelId
        };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}