CREATE TABLE [dbo].[Entity_Reference](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Entity_Sequence_ID] [bigint] NOT NULL,
	[Reference_Type] [varchar](50) NOT NULL,
	[Reference_Value] [nvarchar](255) NOT NULL,
 CONSTRAINT [Entity_Reference_PK] PRIMARY KEY CLUSTERED 
(
	[Entity_Sequence_ID] ASC,
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 50) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Entity_Reference]  WITH CHECK ADD  CONSTRAINT [FK_Entity_Reference_Entity] FOREIGN KEY([Entity_Sequence_ID])
REFERENCES [dbo].[Entity] ([Entity_Sequence_ID])
GO

ALTER TABLE [dbo].[Entity_Reference] CHECK CONSTRAINT [FK_Entity_Reference_Entity]
GO