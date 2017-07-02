USE [DES]
GO

/****** Object:  Table [dbo].[StaffAccess]    Script Date: 1/4/2017 10:12:29 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[StaffAccess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SiteDataId] [int] NOT NULL,
	[StaffAccessIndex] [int] NOT NULL,
	[AccessLevel] [int] NOT NULL,
	[PassNumber] [int] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_StaffAccess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


