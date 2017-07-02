USE [DES]
GO

/****** Object:  Table [dbo].[Resident]    Script Date: 1/4/2017 10:10:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Resident](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SiteId] [int] NOT NULL,
	[FlatNumber] [int] NOT NULL,
	[ResidentName] [varchar](64) NOT NULL,
	[HomeTel] [varchar](64) NOT NULL,
	[MobileTel] [varchar](64) NOT NULL,
	[Email] [varchar](64) NOT NULL,
	[TagIndex] [int] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Resident] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


