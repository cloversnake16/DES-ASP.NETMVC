﻿USE [DES]
GO

/****** Object:  Table [dbo].[UserLogin]    Script Date: 1/23/2017 10:42:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserLogin](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_UserLogin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

