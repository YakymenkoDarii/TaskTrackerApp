using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly string frontendUrl;
    private readonly string priceId;

    public PaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
        frontendUrl = configuration["BaseUri"];
        priceId = configuration["Stripe:PriceId"];
    }

    public async Task<string> CreateCheckoutSessionAsync(User user)
    {
        if (user == null) throw new Exception("User not found");

        var options = new Stripe.Checkout.SessionCreateOptions
        {
            SuccessUrl = frontendUrl + "/subscription/success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = frontendUrl + "/subscription/cancel",
            Mode = "subscription",
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                Price = priceId,
                Quantity = 1,
            },
        },
            Metadata = new Dictionary<string, string>
        {
            {"UserEmail", user.Email},
        }
        };

        if (!string.IsNullOrEmpty(user.StripeCustomerId))
        {
            options.Customer = user.StripeCustomerId;
        }
        else
        {
            var customerService = new Stripe.CustomerService();
            var existingCustomers = await customerService.ListAsync(new Stripe.CustomerListOptions
            {
                Email = user.Email,
                Limit = 1
            });

            if (existingCustomers.Data.Any())
            {
                options.Customer = existingCustomers.Data.First().Id;
            }
            else
            {
                options.CustomerEmail = user.Email;
            }
        }

        var service = new Stripe.Checkout.SessionService();
        Stripe.Checkout.Session session = await service.CreateAsync(options);
        return session.Url;
    }

    public async Task<string> CreatePortalSessionAsync(string stripeCustomerId)
    {
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = stripeCustomerId,
            ReturnUrl = frontendUrl + "/",
        };
        var service = new Stripe.BillingPortal.SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }
}