USE [DES]
GO

/****** Object:  Table [dbo].[BackTask]    Script Date: 1/23/2017 10:19:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[BackTask](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[DeviceId] [varchar](32) NOT NULL,
	[SiteDataId] [int] NOT NULL,
	[WorkType] [varchar](32) NOT NULL,
	[WorkStatus] [varchar](32) NOT NULL,
	[WorkItem] [varchar](32) NOT NULL,
	[WorkIndex] [int] NOT NULL,
	[Description] [varchar](2048) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_BackWorkTask] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


