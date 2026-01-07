using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Columns.Commands.CreateColumns;
using TaskTrackerApp.Application.Features.Columns.Commands.DeleteColumns;
using TaskTrackerApp.Application.Features.Columns.Commands.UpdateColumns;
using TaskTrackerApp.Application.Features.Columns.Queries;
using TaskTrackerApp.Domain.DTOs.Column;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ColumnsController : ControllerBase
{
    private IMediator _mediator;

    public ColumnsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateColumnDto columnDto)
    {
        var command = new CreateColumnCommand
        {
            Title = columnDto.Title,
            Description = columnDto.Description,
            BoardId = columnDto.BoardId,
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateColumnDto columnDto)
    {
        var command = new UpdateColumnCommand
        {
            Id = id,
            Title = columnDto.Title,
            Description = columnDto.Description,
            BoardId = columnDto.BoardId,
            UpdatedById = columnDto.UpdatedById,
            Position = columnDto.Position
        };

        var result = await _mediator.Send(command);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var command = new DeleteColumnCommand
        {
            Id = id
        };

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpGet("{boardId}")]
    public async Task<IActionResult> GetByBoardIdAsync(int boardId)
    {
        var query = new GetColumnsByBoardIdQuery(boardId);

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}