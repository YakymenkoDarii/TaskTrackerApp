namespace TaskTrackerApp.Domain.Settings;

public class StripeSettings
{
    public string SecretKey { get; set; }

    public string WebhookSecret { get; set; }

    public string PriceId { get; set; }
}