-- Add your test scenario here --

CREATE TABLE [dbo].[_Error](
	[Status] [varchar](5) NOT NULL,
	[When] [datetime] NOT NULL CONSTRAINT [DF__Error_When]  DEFAULT (getdate()),
	[Description] [varchar](1000) NOT NULL
) ON [PRIMARY]