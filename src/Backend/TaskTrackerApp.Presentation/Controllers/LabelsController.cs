using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateLabelAsync([FromBody] LabelDto updateDto)
    {
    }

    [HttpGet("{boardId}")]
    public async Task<IActionResult> GetLabelsByBoardIdAsync(int boardId)
    {
    }

    [HttpPost("{cardId}/{labelId}")]
    public async Task<IActionResult> AddLabelToCardAsync(int cardId, int labelId)
    {
    }

    [HttpDelete("{cardId}/{labelId}")]
    public async Task<IActionResult>
}