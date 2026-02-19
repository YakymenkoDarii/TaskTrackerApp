using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Application.Features.Subscription.Command.CreateCheckoutSession;
using TaskTrackerApp.Application.Features.Subscription.Command.CreateCustomerPortal;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckoutSession()
    {
        var command = new CreateCheckoutSessionCommand();

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("portal")]
    public async Task<IActionResult> CreatePortalSession()
    {
        var command = new CreateCustomerPortalCommand();

        var result = await _mediator.Send(command);

        return Ok(result);
    }
}