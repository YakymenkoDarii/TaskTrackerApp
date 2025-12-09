CREATE TABLE Cards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DueDate DATETIME2 NULL,
    
    BoardId INT NOT NULL,
    ColumnId INT NOT NULL,
    AssigneeId INT NULL,

    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy INT NOT NULL,
    UpdatedBy INT NULL,

    CONSTRAINT FK_Cards_Board FOREIGN KEY (BoardId) REFERENCES Boards(Id),
    CONSTRAINT FK_Cards_Column FOREIGN KEY (ColumnId) REFERENCES Columns(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Cards_Assignee FOREIGN KEY (AssigneeId) REFERENCES Users(Id),
    CONSTRAINT FK_Cards_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);