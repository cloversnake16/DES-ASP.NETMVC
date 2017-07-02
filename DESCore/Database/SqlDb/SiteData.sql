USE [DES]
GO

/****** Object:  Table [dbo].[SiteData]    Script Date: 1/27/2017 9:44:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SiteData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SiteId] [int] NOT NULL,
	[SiteDataType] [varchar](64) NOT NULL,
	[TemplateName] [varchar](64) NOT NULL,
	[IsCompleted] [bit] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_SiteData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


