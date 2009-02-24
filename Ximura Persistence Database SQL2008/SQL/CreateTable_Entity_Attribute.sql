CREATE TABLE [dbo].[Entity_Attribute](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Entity_Sequence_ID] [bigint] NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[SubType] [varchar](50) NULL,
	[Language] [varchar](10) NOT NULL,
	[Value] [nvarchar](3000) NULL,
 CONSTRAINT [Entity_Attribute_PK] PRIMARY KEY CLUSTERED 
(
	[Entity_Sequence_ID] ASC,
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 50) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Entity_Attribute]  WITH CHECK ADD  CONSTRAINT [FK_Entity_Attribute_Entity] FOREIGN KEY([Entity_Sequence_ID])
REFERENCES [dbo].[Entity] ([Entity_Sequence_ID])
GO

ALTER TABLE [dbo].[Entity_Attribute] CHECK CONSTRAINT [FK_Entity_Attribute_Entity]
GO