USE [jud12.flcourts.org]
GO

/****** Object:  Table [dbo].[tjc_car_application_by_jac_code]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_application_by_jac_code](
	[JacCodeId] [int] NOT NULL,
	[LocationId] [int] NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_tjc_car_application_by_jac_code] PRIMARY KEY CLUSTERED 
(
	[JacCodeId] ASC,
	[LocationId] ASC,
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_applications]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_applications](
	[ApplicationId] [int] IDENTITY(1,1) NOT NULL,
	[AttorneyId] [int] NULL,
	[Year] [int] NULL,
	[Status] [int] NULL,
	[DateCreated] [datetime] NULL,
	[DateReviewed] [datetime] NULL,
	[DateOfPeriod] [datetime] NULL,
	[IsRenewal] [bit] NULL,
	[RemoteContactInfo] [nvarchar](max) NULL,
	[YearsOnRegistry] [int] NULL,
	[RejectionText] [nvarchar](max) NULL,
	[CertSignature] [nvarchar](50) NULL,
	[GuardianSignature] [nvarchar](50) NULL,
	[Exported] [bit] NULL,
	[ExportDate] [datetime] NULL,
 CONSTRAINT [PK_tjc_car_applications] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_attorneys]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_attorneys](
	[AttorneyId] [int] IDENTITY(1,1) NOT NULL,
	[BarNumber] [int] NULL,
	[LastName] [nvarchar](25) NULL,
	[FirstName] [nvarchar](25) NULL,
	[Address] [nvarchar](150) NULL,
	[City] [nvarchar](50) NULL,
	[State] [char](2) NULL,
	[Zip] [nvarchar](10) NULL,
	[Email] [nvarchar](250) NULL,
	[Phone] [nvarchar](20) NULL,
	[Cell] [nvarchar](20) NULL,
	[Fax] [nvarchar](20) NULL,
	[Language] [nvarchar](50) NULL,
	[LawFirm] [nvarchar](100) NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_tjc_car_attorneys] PRIMARY KEY CLUSTERED 
(
	[AttorneyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_case_types]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_case_types](
	[CaseTypeID] [int] IDENTITY(1,1) NOT NULL,
	[CaseTypeName] [nvarchar](50) NULL,
	[LabelNote] [nvarchar](50) NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_tjc_car_case_type] PRIMARY KEY CLUSTERED 
(
	[CaseTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_current_periods]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_current_periods](
	[ApplicationYear] [int] NOT NULL,
	[AcceptingNewApplications] [bit] NULL,
	[ModificationDeadline] [datetime] NULL,
 CONSTRAINT [PK_tjc_car_current_periods] PRIMARY KEY CLUSTERED 
(
	[ApplicationYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_jac_code_config]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_jac_code_config](
	[JacCodeId] [int] NOT NULL,
	[LocationId] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[Exclude] [bit] NULL,
	[OnlyRenewals] [bit] NULL,
 CONSTRAINT [PK_tjc_car_jac_code_config] PRIMARY KEY CLUSTERED 
(
	[JacCodeId] ASC,
	[LocationId] ASC,
	[Year] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_jac_codes]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_jac_codes](
	[JacCodeId] [int] NOT NULL,
	[Category] [nvarchar](150) NULL,
	[LabelNote] [nvarchar](50) NULL,
	[CaseTypeId] [int] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_tjc_car_case_types_1] PRIMARY KEY CLUSTERED 
(
	[JacCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_jac_codes_updates]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_jac_codes_updates](
	[JacCodeId] [int] NOT NULL,
	[Category] [nvarchar](150) NULL,
	[CaseTypeId] [int] NULL,
	[UpdateType] [int] NULL,
 CONSTRAINT [PK_car_jac_codes_updates] PRIMARY KEY CLUSTERED 
(
	[JacCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_locations]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_locations](
	[LocationId] [int] IDENTITY(1,1) NOT NULL,
	[Abbreviation] [nvarchar](10) NULL,
	[LocationName] [nvarchar](50) NULL,
	[CountyNumber] [int] NULL,
 CONSTRAINT [PK_tjc_car_locations] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_registry]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_registry](
	[RegistryId] [int] IDENTITY(1,1) NOT NULL,
	[AttorneyId] [int] NULL,
	[ApplicationId] [int] NULL,
	[DateApproved] [datetime] NULL,
 CONSTRAINT [PK_tjc_car_registry] PRIMARY KEY CLUSTERED 
(
	[RegistryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tjc_car_settings]    Script Date: 5/11/2026 11:21:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tjc_car_settings](
	[ID] [int] NOT NULL,
	[VerificationNote] [nvarchar](max) NULL,
	[EditAttorneyNote] [nvarchar](max) NULL,
	[EditApplicationNote] [nvarchar](max) NULL,
	[ApplicationEmail] [nvarchar](max) NULL,
	[UpdateNotificationSendTo] [nvarchar](500) NULL,
	[ContactEmail] [nvarchar](250) NULL,
	[BeginFiscalYearMonth] [int] NULL,
	[BeginFiscalYearDay] [int] NULL,
 CONSTRAINT [PK_tjc_car_settings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[tjc_car_applications] ADD  CONSTRAINT [DF_tjc_car_applications_Exported]  DEFAULT ((1)) FOR [Exported]
GO

ALTER TABLE [dbo].[tjc_car_case_types] ADD  CONSTRAINT [DF_tjc_car_case_type_Active]  DEFAULT ((0)) FOR [Active]
GO

ALTER TABLE [dbo].[tjc_car_jac_code_config] ADD  CONSTRAINT [DF_tjc_car_jac_code_config_Exclude]  DEFAULT ((0)) FOR [Exclude]
GO

ALTER TABLE [dbo].[tjc_car_jac_code_config] ADD  CONSTRAINT [DF_tjc_car_jac_code_config_OnlyRenewals]  DEFAULT ((0)) FOR [OnlyRenewals]
GO

ALTER TABLE [dbo].[tjc_car_jac_codes] ADD  CONSTRAINT [DF_tjc_car_case_types_Active]  DEFAULT ((0)) FOR [Active]
GO

ALTER TABLE [dbo].[tjc_car_case_types]  WITH CHECK ADD  CONSTRAINT [FK_tjc_car_case_type_tjc_car_case_type] FOREIGN KEY([CaseTypeID])
REFERENCES [dbo].[tjc_car_case_types] ([CaseTypeID])
GO

ALTER TABLE [dbo].[tjc_car_case_types] CHECK CONSTRAINT [FK_tjc_car_case_type_tjc_car_case_type]
GO

ALTER TABLE [dbo].[tjc_car_jac_codes]  WITH CHECK ADD  CONSTRAINT [FK_tjc_car_jac_codes_tjc_car_case_type] FOREIGN KEY([CaseTypeId])
REFERENCES [dbo].[tjc_car_case_types] ([CaseTypeID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tjc_car_jac_codes] CHECK CONSTRAINT [FK_tjc_car_jac_codes_tjc_car_case_type]
GO


