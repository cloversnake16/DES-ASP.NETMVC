USE [DES]
GO

/****** Object:  Table [dbo].[RemoteSiteEventLog]    Script Date: 1/4/2017 10:10:17 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[RemoteSiteEventLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SiteId] [int] NULL,
	[Status] [varchar](32) NULL,
	[EventNumber] [int] NOT NULL,
	[DateEvent] [datetime] NOT NULL,
	[DateACM] [datetime] NOT NULL,
	[Description] [varchar](512) NULL,
	[Request] [varbinary](max) NULL,
	[Response] [varbinary](max) NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_RemoteSiteEventLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


