USE [DES]
GO

/****** Object:  Table [dbo].[Settings]    Script Date: 1/4/2017 10:11:36 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Settings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CheckPeriod] [int] NOT NULL,
	[MaxDataSize] [int] NOT NULL,
	[MaxEventlogSize] [int] NOT NULL,
	[DateReset] [date] NOT NULL,
	[ReservedChannel] [int] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


