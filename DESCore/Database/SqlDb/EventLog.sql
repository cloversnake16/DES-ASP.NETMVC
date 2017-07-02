USE [DES]
GO

/****** Object:  Table [dbo].[EventLog]    Script Date: 1/4/2017 10:09:51 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[EventLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SiteId] [int] NULL,
	[Event] [varchar](32) NULL,
	[Status] [varchar](32) NULL,
	[Description] [varchar](512) NULL,
	[Request] [varbinary](max) NULL,
	[Response] [varbinary](max) NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_EventLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


