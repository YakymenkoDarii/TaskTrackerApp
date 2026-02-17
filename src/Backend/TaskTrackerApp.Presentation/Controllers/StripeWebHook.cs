using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using TaskTrackerApp.Application.Features.Subscription.Command.FulfilSubscription;
using TaskTrackerApp.Application.Features.Subscription.Command.HandlePaymentFailure;
using TaskTrackerApp.Application.Features.Subscription.Command.RevokeSubscription;
using TaskTrackerApp.Domain.Settings;

namespace TaskTrackerApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeWebHook : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly string _webhookSecret;

    public StripeWebHook(IMediator mediator, IOptions<StripeSettings> settings)
    {
        _mediator = mediator;
        _webhookSecret = settings.Value.WebhookSecret;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var signatureHeader = Request.Headers["Stripe-Signature"];
            var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _webhookSecret);

            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    var session = stripeEvent.Data.Object as Session;
                    var subscriptionService = new SubscriptionService();

                    Subscription userSubscription = await subscriptionService.GetAsync(session.SubscriptionId);

                    var firstItem = userSubscription.Items.Data[0];

                    await _mediator.Send(new FulfillSubscriptionCommand
                    {
                        UserEmail = session.Metadata.TryGetValue("UserEmail", out var email)
                                    ? email
                                    : session.CustomerDetails.Email,
                        StripeCustomerId = session.CustomerId,
                        SubscriptionEndDate = firstItem.CurrentPeriodEnd
                    });
                    break;

                case EventTypes.CustomerSubscriptionDeleted:
                    var subscription = stripeEvent.Data.Object as Subscription;

                    await _mediator.Send(new RevokeSubscriptionCommand
                    {
                        StripeCustomerId = subscription.CustomerId
                    });
                    break;

                case EventTypes.InvoicePaymentFailed:
                    var invoice = stripeEvent.Data.Object as Invoice;

                    await _mediator.Send(new HandlePaymentFailureCommand
                    {
                        StripeCustomerId = invoice.CustomerId,
                        FailureReason = $"Payment failed for Invoice {invoice.Id}"
                    });
                    break;

                default:
                    Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                    break;
            }

            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine($"Stripe Error: {e.Message}");
            return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Server Error: {e.Message}");
            return StatusCode(500);
        }
    }
}