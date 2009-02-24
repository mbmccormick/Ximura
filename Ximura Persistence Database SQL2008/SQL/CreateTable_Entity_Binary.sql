CREATE TABLE [dbo].[Entity_Binary](
	[Entity_Sequence_ID] [bigint] NOT NULL,
	[Image_Value] [image] NULL,
 CONSTRAINT [Entity_Attribute_Binary_PK] PRIMARY KEY CLUSTERED 
(
	[Entity_Sequence_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 50) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Entity_Binary]  WITH CHECK ADD  CONSTRAINT [FK_Entity_Binary_Entity] FOREIGN KEY([Entity_Sequence_ID])
REFERENCES [dbo].[Entity] ([Entity_Sequence_ID])
GO

ALTER TABLE [dbo].[Entity_Binary] CHECK CONSTRAINT [FK_Entity_Binary_Entity]
GO


