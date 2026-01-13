CREATE TABLE [dbo].[BoardInvitations] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [BoardId] INT NOT NULL,
    [SenderId] INT NOT NULL,
    [InviteeEmail] NVARCHAR(256) NOT NULL,
    [InviteeId] INT NULL,
    
    [Role] NVARCHAR(50) NOT NULL, 
    
    [Status] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_BoardInvitations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BoardInvitations_Boards_BoardId] FOREIGN KEY ([BoardId]) 
        REFERENCES [dbo].[Boards] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BoardInvitations_Users_SenderId] FOREIGN KEY ([SenderId]) 
        REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BoardInvitations_Users_InviteeId] FOREIGN KEY ([InviteeId]) 
        REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
);