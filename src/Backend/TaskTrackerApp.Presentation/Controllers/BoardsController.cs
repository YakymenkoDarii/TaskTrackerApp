using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;
using TaskTrackerApp.Application.Features.Boards.Commands.DeleteBoards;
using TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoards;
using TaskTrackerApp.Application.Features.Boards.Queries.GetAllBoards;
using TaskTrackerApp.Application.Features.Boards.Queries.GetBoardById;
using TaskTrackerApp.Domain.DTOs.Board;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoardsController : ControllerBase
{
    private IMediator _mediator;

    public BoardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("boards")]
    public async Task<IActionResult> GetAllAsync()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdString, out int userId))
        {
            return Unauthorized("Invalid Token: User ID is missing or not a number.");
        }

        var query = new GetAllBoardsQuery(userId);
        var result = await _mediator.Send(query);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{boardId}")]
    public async Task<IActionResult> GetByIdAsync(int boardId)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }
        var query = new GetBoardByIdQuery
        {
            Id = boardId,
            CurrentUserId = userId
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            if (result.Error?.Code == "Unauthorized")
                return StatusCode(403, result.Error);

            if (result.Error?.Code == "NotFound")
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBoardDto boardDto)
    {
        var command = new CreateBoardCommand
        {
            Title = boardDto.Title,
            Description = boardDto.Description,
            CreatedById = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
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
            UpdatedById = boardDto.UpdatedById
        };

        await _mediator.Send(command);

        return Ok(command);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var command = new DeleteBoardCommand
        {
            Id = id
        };

        await _mediator.Send(command);

        return NoContent();
    }
}