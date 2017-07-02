USE [DES]
GO

/****** Object:  Table [dbo].[Staff]    Script Date: 1/23/2017 10:42:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Staff](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StaffName] [varchar](32) NOT NULL,
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
 CONSTRAINT [PK_Staff] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


