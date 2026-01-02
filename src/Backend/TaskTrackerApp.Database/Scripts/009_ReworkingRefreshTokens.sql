DROP TABLE dbo.RefreshTokens;
ALTER TABLE Users
ADD RefreshToken NVARCHAR(MAX) NULL,
    RefreshTokenExpiration DATETIME2 NULL;