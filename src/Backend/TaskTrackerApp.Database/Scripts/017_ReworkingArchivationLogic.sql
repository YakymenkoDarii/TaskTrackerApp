EXEC sp_rename 'dbo.Boards.IsBackedUp', 'IsQueuedForArchival', 'COLUMN';

CREATE TABLE ArchivedBoards(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Title NVARCHAR(200) NOT NULL,
	Description NVARCHAR(MAX) NULL,
	OriginalBoardId INT NOT NULL);

CREATE TABLE ArchivedBoardMembers(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	UserId INT NOT NULL,
	ArchivedBoardId INT NOT NULL,
	Role NVARCHAR(50) NULL,

	CONSTRAINT FK_ArchivedBoardMembers_UserId FOREIGN KEY (UserId) REFERENCES Users(Id),
	CONSTRAINT FK_ArchivedBoardMembers_ArchiveBoardId FOREIGN KEY (ArchivedBoardId) REFERENCES ArchivedBoards(Id)
);