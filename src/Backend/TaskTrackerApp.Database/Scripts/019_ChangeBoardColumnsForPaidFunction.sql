IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'LastTimeOpenned' AND Object_ID = OBJECT_ID(N'dbo.Boards'))
BEGIN
    EXEC sp_rename 'dbo.Boards.LastTimeOpenned', 'LastModified', 'COLUMN';
END

DECLARE @ConstraintName nvarchar(200);

SELECT @ConstraintName = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE dc.parent_object_id = OBJECT_ID('Boards') 
  AND c.name = 'Position';

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Boards DROP CONSTRAINT ' + @ConstraintName);
END

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Position' AND Object_ID = Object_ID(N'Boards'))
BEGIN
    ALTER TABLE Boards DROP COLUMN Position;
END