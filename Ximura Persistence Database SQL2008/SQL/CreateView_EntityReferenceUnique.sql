CREATE VIEW [dbo].[View_EntityReferenceUnique]
AS
SELECT    e.Entity_Type_ID
		, er.Reference_Type
		, er.Reference_Value
FROM dbo.Entity_Reference er
INNER JOIN dbo.Entity e ON er.Entity_Sequence_ID = e.Entity_Sequence_ID


GO