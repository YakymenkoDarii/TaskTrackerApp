using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Faq.Commands;
using TaskTrackerApp.Application.Features.Faq.Commands.AskQuestion;
using TaskTrackerApp.Application.Features.Faq.Queries.GetHistory;
using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FaqController : ControllerBase
{
    private readonly IMediator _mediator;

    public FaqController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedWithDataAsync()
    {
        var command = new SeedFaqDataCommand();

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskAsync([FromBody] ChatRequest request)
    {
        var command = new AskQuestionCommand
        {
            SessionId = request.SessionId,
            Question = request.Question
        };

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("history/{sessionId}")]
    public async Task<IActionResult> GetHistoryAsync(string sessionId)
    {
        var query = new GetChatHistoryQuery
        {
            SessionId = sessionId
        };

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}