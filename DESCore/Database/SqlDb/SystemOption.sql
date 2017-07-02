USE [DES]
GO

/****** Object:  Table [dbo].[SystemOption]    Script Date: 1/4/2017 10:12:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SystemOption](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SiteDataId] [int] NOT NULL,
	[Option1] [int] NOT NULL,
	[Option2] [int] NOT NULL,
	[TradeSchedule] [int] NOT NULL,
	[RingTimeout] [int] NOT NULL,
	[AudioTimeout] [int] NOT NULL,
	[WardenChannel] [int] NOT NULL,
	[CustomerNo] [int] NOT NULL,
	[SiteNo] [int] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_SystemOption] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


