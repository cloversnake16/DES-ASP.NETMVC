USE [DES]
GO

/****** Object:  Table [dbo].[User]    Script Date: 1/4/2017 10:13:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](32) NOT NULL,
	[DefaultPassword] [varchar](32) NOT NULL,
	[Password] [varchar](256) NULL,
	[UserTypeId] [int] NOT NULL,
	[FirstName] [varchar](64) NULL,
	[LastName] [varchar](64) NULL,
	[Address] [varchar](64) NULL,
	[Email] [varchar](64) NOT NULL,
	[ContactNumber] [varchar](32) NULL,
	[IsLogin] [bit] NOT NULL,
	[ViewP] [char](1) NULL,
	[EditP] [char](1) NULL,
	[ReadP] [char](1) NULL,
	[WriteP] [char](1) NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_ToUserType] FOREIGN KEY([UserTypeId])
REFERENCES [dbo].[UserType] ([Id])
GO

ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_ToUserType]
GO


