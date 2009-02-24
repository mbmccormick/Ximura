CREATE TABLE [dbo].[Entity](
	[Entity_Sequence_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Entity_Version_ID] [uniqueidentifier] NOT NULL,
	[Entity_Content_ID] [uniqueidentifier] NOT NULL,
	[Entity_Type_ID] [uniqueidentifier] NOT NULL,
	[Assembly_Qualified_Name] [varchar](255) NOT NULL,
	[Create_Date] [datetime] NOT NULL,
	[Built] [bit] NULL,
	[Expiry_Date_Time] [datetime] NULL,
	[Create_By] [uniqueidentifier] NULL,
	[Create_By_Reference_ID_Code] [nvarchar](50) NULL,
	[Status] [varchar](10) NOT NULL,
	[Update_Date] [datetime] NULL,
 CONSTRAINT [PK_Entity] PRIMARY KEY CLUSTERED 
(
	[Entity_Sequence_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]