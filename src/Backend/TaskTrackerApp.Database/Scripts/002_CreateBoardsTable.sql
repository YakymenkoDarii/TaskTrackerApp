CREATE TABLE Boards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy INT NOT NULL,
    UpdatedBy INT NULL,

    CONSTRAINT FK_Boards_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    CONSTRAINT FK_Boards_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
);