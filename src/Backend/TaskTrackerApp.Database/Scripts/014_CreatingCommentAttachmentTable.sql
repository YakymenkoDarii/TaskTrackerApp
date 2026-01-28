CREATE TABLE CommentAttachments(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FileName NVARCHAR(255) NOT NULL,
    StoredFileName NVARCHAR(100) NOT NULL,
    ContentType  NVARCHAR(100) NOT NULL,
    Url NVARCHAR(500) NOT NULL,
    Size BIGINT NOT NULL,
    CommentId INT NOT NULL,

    CONSTRAINT FK_CommentAttachments_CommentId FOREIGN KEY (CommentId) REFERENCES CardComments(Id) ON DELETE CASCADE
);