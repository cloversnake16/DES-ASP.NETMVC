USE [DES]
GO

/****** Object:  Table [dbo].[Door]    Script Date: 1/4/2017 10:09:36 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Door](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SiteDataId] [int] NOT NULL,
	[DoorIndex] [int] NOT NULL,
	[LockTimeout] [int] NOT NULL,
	[ScheduleIndex] [int] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Door] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


