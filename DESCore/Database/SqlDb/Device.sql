USE [DES]
GO

/****** Object:  Table [dbo].[Device]    Script Date: 1/23/2017 10:40:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Device](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceId] [varchar](32) NOT NULL,
	[PhoneNumber] [varchar](32) NOT NULL,
	[IpAddress] [varchar](32) NOT NULL,
	[ACMVersion] [varchar](32) NOT NULL,
	[InBound] [bigint] NOT NULL,
	[OutBound] [bigint] NOT NULL,
	[IsConnect] [bit] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


