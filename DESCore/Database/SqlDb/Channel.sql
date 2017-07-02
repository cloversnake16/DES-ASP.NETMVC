USE [DES]
GO

/****** Object:  Table [dbo].[Channel]    Script Date: 1/4/2017 10:08:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Channel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SiteDataId] [int] NOT NULL,
	[ChannelIndex] [int] NOT NULL,
	[Flat] [int] NOT NULL,
	[PPP] [varchar](32) NOT NULL,
	[Door1] [bigint] NOT NULL,
	[Door2] [bigint] NOT NULL,
	[Tag1] [int] NOT NULL,
	[Tag2] [int] NOT NULL,
	[Tag3] [int] NOT NULL,
	[Tag4] [int] NOT NULL,
	[Tag5] [int] NOT NULL,
	[Tag6] [int] NOT NULL,
	[Tag7] [int] NOT NULL,
	[Tag8] [int] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Channel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


