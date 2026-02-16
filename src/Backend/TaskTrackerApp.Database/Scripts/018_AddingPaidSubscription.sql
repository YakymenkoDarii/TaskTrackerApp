ALTER TABLE Users
ADD IsPro BIT NOT NULL DEFAULT 0,
StripeCustomerId NVARCHAR(50) NULL,
SubscriptionEndDate DATETIME2 NULL;

CREATE UNIQUE INDEX IX_Users_StripeCustomerId 
ON Users(StripeCustomerId)
WHERE StripeCustomerId IS NOT NULL;