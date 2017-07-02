USE [DES]
GO

/****** Object:  Table [dbo].[Schedule]    Script Date: 1/4/2017 10:11:12 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Schedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SiteDataId] [int] NOT NULL,
	[ScheduleIndex] [int] NOT NULL,
	[Start1Hour] [int] NOT NULL,
	[Start1Minute] [int] NOT NULL,
	[End1Hour] [int] NOT NULL,
	[End1Minute] [int] NOT NULL,
	[Day1] [int] NOT NULL,
	[Start2Hour] [int] NOT NULL,
	[Start2Minute] [int] NOT NULL,
	[End2Hour] [int] NOT NULL,
	[End2Minute] [int] NOT NULL,
	[Day2] [int] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


