using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;
using TaskTrackerApp.Application.Features.Boards.Commands.DeleteBoards;
using TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoards;
using TaskTrackerApp.Application.Features.Boards.Queries.GetAllBoards;
using TaskTrackerApp.Application.Features.Boards.Queries.GetBoardById;
using TaskTrackerApp.Domain.DTOs.Board;

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
        var query = new GetAllBoardsQuery(userId);

        var result = await _mediator.Send(query);

        return result is null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetById(int boardId)
    {
        var query = new GetBoardByIdQuery(boardId);

        var result = await _mediator.Send(query);

        return result is null
            ? NotFound()
            : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBoardDto boardDto)
    {
        var command = new CreateBoardCommand
        {
            Title = boardDto.Title,
            Description = boardDto.Description,
            CreatedBy = boardDto.CreatedBy
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateBoardDto boardDto)
    {
        var command = new UpdateBoardCommand
        {
            Id = id,
            Title = boardDto.Title,
            Description = boardDto.Description,
            UpdatedBy = boardDto.UpdatedBy
        };

        await _mediator.Send(command);

        return Ok(command);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBoard(int id)
    {
        var command = new DeleteBoardCommand
        {
            Id = id
        };

        await _mediator.Send(command);

        return NoContent();
    }
}