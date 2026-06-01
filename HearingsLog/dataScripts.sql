
/****** Object:  Table [dbo].[tjc_hearing_jacs_judges]    Script Date: 9/9/2025 1:56:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tjc_hearing_jacs_judges](
	[JacsUserID] [int] IDENTITY(1,1) NOT NULL,
	[JudgeID] [nvarchar](20) NULL,
	[County] [nvarchar](50) NULL,
	[JudgeName] [nvarchar](200) NULL,
 CONSTRAINT [PK_tjc_hearing_jacs_judges] PRIMARY KEY CLUSTERED 
(
	[JacsUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tjc_hearing_jacs_userid_by_userid]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tjc_hearing_jacs_userid_by_userid](
	[JACSUserID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
 CONSTRAINT [PK_tjc_hearing_jacs_userid_by_userid] PRIMARY KEY CLUSTERED 
(
	[JACSUserID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tjc_hearing_judge_ja]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tjc_hearing_judge_ja](
	[JudgeUserID] [int] NOT NULL,
	[JaUserID] [int] NOT NULL,
 CONSTRAINT [PK_tjc_hearing_judge_ja] PRIMARY KEY CLUSTERED 
(
	[JudgeUserID] ASC,
	[JaUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tjc_hearing_log]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tjc_hearing_log](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[CalendarID] [int] NULL,
	[County] [nvarchar](50) NULL,
	[OrderSigned] [datetime] NULL,
	[HearingDate] [datetime] NULL,
	[CaseName] [nvarchar](500) NULL,
	[CaseNumber] [nvarchar](200) NULL,
	[DIN] [nvarchar](200) NULL,
	[MotionTitle] [nvarchar](500) NULL,
	[DraftedBy] [nvarchar](500) NULL,
	[CourtNotes] [nvarchar](max) NULL,
	[DelayReason] [nvarchar](max) NULL,
	[JudgeID] [nvarchar](20) NULL,
	[Status] [int] NULL,
	[CreatedByID] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[LastModifiedDate] [datetime] NULL,
	[LastModifiedByID] [int] NULL,
 CONSTRAINT [PK_tjc_hearing_log] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[tjc_hearing_cc]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[tjc_hearing_cc]
AS
SELECT        h.RequestorName AS JudgeName, h.RequestorId AS JudgeID, h.MotionFiled, h.CaseNumber, h.PartyName AS CaseName, h.Description, h.StatusName AS CaseStatus, h.Responsible AS Attorney, h.County, h.DateReceived, 
                         h.logId, h.CaseType, h.DateCompleted, ref.UserID
FROM            intranet.dbo.tjc_hearing_court_counsel AS h INNER JOIN
                         dbo.tjc_hearing_jacs_judges AS jj ON h.RequestorId = jj.JudgeID INNER JOIN
                         dbo.tjc_hearing_jacs_userid_by_userid AS ref ON ref.JACSUserID = jj.JacsUserID
WHERE        (jj.County = 'CourtCounsel')
GO
/****** Object:  View [dbo].[tjc_hearing_cc_judges]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[tjc_hearing_cc_judges]
AS
SELECT        RequestorId AS JudgeID, RequestorName AS JudgeName, IsActive
FROM            intranet.dbo.aws_cc_Requestor
GO
ALTER TABLE [dbo].[tjc_hearing_log] ADD  CONSTRAINT [DF_tjc_hearing_log_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tjc_hearing_log] ADD  CONSTRAINT [DF_tjc_hearing_log_LastModifiedDate]  DEFAULT (getdate()) FOR [LastModifiedDate]
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_create_jacs_judge_by_user_ref]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_create_jacs_judge_by_user_ref] 
	@jacsJudge int, @userId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO tjc_hearing_jacs_userid_by_userid(JACSUserID,UserID)
	VALUES (@jacsJudge,@userId);
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_create_judge_ja_ref]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_create_judge_ja_ref] 
	@judgeUserId int, @jaUserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO tjc_hearing_judge_ja(JudgeUserID,JaUserID)
	VALUES (@judgeUserId,@jaUserId);
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_delete_jacs_judge_by_user_ref]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Deletes JACS Judge to UserID xref>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_delete_jacs_judge_by_user_ref] 
	@jacsJudge int, @userId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete  from tjc_hearing_jacs_userid_by_userid 
	Where JacsUserID =@jacsJudge and UserID=@userId  ;
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_delete_jacs_judges_by_user_ref]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Deletes JACS Judge to UserID xref>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_delete_jacs_judges_by_user_ref] 
	@userId int,@county nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete x from tjc_hearing_jacs_userid_by_userid x inner join tjc_hearing_jacs_judges j on x.JACSUserID=j.JacsUserID
	Where UserID =@userId and j.County= @county;
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_delete_judge_ja_ref]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_delete_judge_ja_ref] 
	@judgeUserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM tjc_hearing_judge_ja WHERE JudgeUserID = @judgeUserId 
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_county_jacs_judges]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Deletes JACS Judge to UserID xref>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_county_jacs_judges] 
	 @county nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   Select * from tjc_hearing_jacs_judges
   Where County=@county;
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_existing_county_jacs_judges]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Deletes JACS Judge to UserID xref>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_existing_county_jacs_judges] 
	 @county nvarchar(50),@userId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   Select x.JacsUserID,x.UserID,u.DisplayName as Name from tjc_hearing_jacs_judges jj inner join tjc_hearing_jacs_userid_by_userid x on jj.JacsUserID=x.JACSUserID inner join Users u on u.UserID=x.UserID
   Where County=@county and x.UserID<>@UserId;
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_ja_judge_ref]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_ja_judge_ref] 
	@jaUserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select top 1 * FROM tjc_hearing_judge_ja WHERE JaUserID = @jaUserId 
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_judge_ja_ref]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_judge_ja_ref] 
	@judgeUserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select top 1 * FROM tjc_hearing_judge_ja WHERE JudgeUserID = @judgeUserId 
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_log_count]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/3/2023>
-- Description:	<Parameratized Dynamic Sql for Mediator List Filtering>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_log_count] 
	@userId int,@status int, @startDate datetime, @endDate datetime
AS

BEGIN
	DECLARE @sql nvarchar(MAX)='SELECT Count(*) FROM tjc_hearing_log l WHERE (l.HearingDate between @startDate AND @endDate) AND (Exists(Select j.JudgeID from tjc_hearing_jacs_judges j Inner Join tjc_hearing_jacs_userid_by_userid x on j.JACSUserID = x.JACSUserID WHERE x.UserID=@userId AND j.JudgeId=l.JudgeID and j.County=l.County) OR l.LastModifiedByID =@userId)';
	DECLARE @ParameterDef nvarchar(MAX)
 
    SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime';
							
	SET NOCOUNT ON;
	IF @status >=0
		BEGIN
			SET	@sql = @sql + ' AND l.Status =  @status';
		END
	
    -- Execute parameratized SQL
	EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId,@status=@status,@startDate=@startDate,@endDate=@endDate;

END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_log_count_chief_judge]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/3/2023>
-- Description:	<Parameratized Dynamic Sql for Mediator List Filtering>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_log_count_chief_judge] 
	@userId int,@status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50), @judgeId int
AS
DECLARE @sql nvarchar(MAX)='';
DECLARE @ParameterDef nvarchar(MAX)

BEGIN
	IF ISNULL(@searchText,'') != '' AND @judgeId > 0
			BEGIN
			SET @searchText = '%' + @searchText + '%';
			SET @sql ='SELECT Count(*) FROM tjc_hearing_log l WHERE (l.HearingDate BETWEEN @startDate AND @endDate) AND (casename like @searchText OR casenumber like @searchText OR din like @searchText OR motiontitle like @searchText OR draftedby like @searchText OR delayreason like @searchText OR courtnotes like @searchText) AND l.LastModifiedByID=@judgeId'; 
			SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50), @judgeId int';
			SET NOCOUNT ON;
			IF @status >=0
				BEGIN
					SET	@sql = @sql + ' AND l.Status =  @status';
				END
			-- Execute parameratized SQL
			EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @searchText=@searchText, @judgeId=@judgeId;
		END
	ELSE IF ISNULL(@searchText,'') = '' AND @judgeId > 0
		BEGIN
			SET @sql ='SELECT Count(*) FROM tjc_hearing_log l WHERE (l.HearingDate BETWEEN @startDate AND @endDate) AND l.LastModifiedByID=@judgeId'; 
			SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @judgeId int';
			SET NOCOUNT ON;
			IF @status >=0
				BEGIN
					SET	@sql = @sql + ' AND l.Status =  @status';
				END
			-- Execute parameratized SQL
			EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @judgeId=@judgeId;

		END
	ELSE IF ISNULL(@searchText,'') != '' AND @judgeId = 0
		BEGIN
			SET @searchText = '%' + @searchText + '%';
			SET @sql ='SELECT Count(*) FROM tjc_hearing_log l WHERE l.LastModifiedByID IS NOT NULL AND (l.HearingDate BETWEEN @startDate AND @endDate) AND (casename like @searchText OR casenumber like @searchText OR din like @searchText OR motiontitle like @searchText OR draftedby like @searchText OR delayreason like @searchText OR courtnotes like @searchText)'; 
			SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50)';
			SET NOCOUNT ON;
			IF @status >=0
				BEGIN
					SET	@sql = @sql + ' AND l.Status =  @status';
				END
			-- Execute parameratized SQL
			EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @searchText=@searchText;
		END
	ELSE
		BEGIN
			SET @sql ='SELECT Count(*) FROM tjc_hearing_log l WHERE l.LastModifiedByID IS NOT NULL AND (l.HearingDate BETWEEN @startDate AND @endDate)';
			
			SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime';
			SET NOCOUNT ON;
			IF @status >=0
				BEGIN
					SET	@sql = @sql + ' AND l.Status =  @status';
				END
			-- Execute parameratized SQL
			EXEC sp_Executesql  @sql, @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate;
		END
END

GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_log_count_old]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/3/2023>
-- Description:	<Parameratized Dynamic Sql for Mediator List Filtering>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_log_count_old] 
	@userId int,@status int, @cutoffDate datetime
AS

BEGIN
	DECLARE @sql nvarchar(MAX)='SELECT Count(*) FROM tjc_hearing_log WHERE HearingDate >= @cutoffDate AND JudgeID in (Select j.JudgeID from tjc_hearing_jacs_judges j Inner Join tjc_hearing_jacs_userid_by_userid x on j.JACSUserID = x.JACSUserID WHERE x.UserID=@userId)';
	DECLARE @ParameterDef nvarchar(MAX)
 
    SET @ParameterDef = '@userId int, @status int, @cutoffDate datetime';
							
	SET NOCOUNT ON;
	IF @status >=0
		BEGIN
			SET	@sql = @sql + ' AND Status =  @status';
		END
	
    -- Execute parameratized SQL
	EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId,@status=@status,@cutoffDate=@cutoffDate;

END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_log_count_search]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/3/2023>
-- Description:	<Parameratized Dynamic Sql for Mediator List Filtering>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_log_count_search] 
	@userId int,@status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50)
AS

BEGIN
	SET @searchText = '%' + @searchText + '%';
	DECLARE @sql nvarchar(MAX)='SELECT Count(*) FROM tjc_hearing_log l 
		WHERE ((l.HearingDate BETWEEN @startDate AND @endDate) 
			AND (Exists(Select j.JudgeID from tjc_hearing_jacs_judges j 
				Inner Join tjc_hearing_jacs_userid_by_userid x on j.JACSUserID = x.JACSUserID 
				WHERE x.UserID=@userId AND j.JudgeId=l.JudgeID and j.County=l.County) 
				OR l.LastModifiedByID =@userId)) 
			AND (casename like @searchText OR casenumber like @searchText OR din like @searchText 
			OR motiontitle like @searchText OR draftedby like @searchText 
			OR delayreason like @searchText OR courtnotes like @searchText)';
	DECLARE @ParameterDef nvarchar(MAX)
  
    SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50)';
							
	SET NOCOUNT ON;
	IF @status >=0
		BEGIN
			SET	@sql = @sql + ' AND l.Status =  @status';
		END
	
    -- Execute parameratized SQL
	EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @searchText=@searchText;

END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_log_paged]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Parameratized Dynamic Sql for Mediation Case List Filtering>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_log_paged] 
	@userId int,@status int, @startDate datetime, @endDate datetime, @offset int, @pageSize int,@sortOrder nvarchar(20),@direction nvarchar(6)
AS

BEGIN
	DECLARE @sql nvarchar(MAX)='SELECT l.[LogID]
      ,l.[CalendarID]
      ,l.[County]
      ,l.[OrderSigned]
      ,l.[HearingDate]
      ,l.[CaseName]
      ,l.[CaseNumber]
      ,l.[DIN]
      ,l.[MotionTitle]
      ,l.[DraftedBy]
      ,l.[CourtNotes]
      ,l.[DelayReason]
      ,ISNULL(u.displayname,l.[JudgeID]) as JudgeID
      ,l.[Status]
      ,l.[CreatedByID]
      ,l.[CreatedDate]
      ,l.[LastModifiedDate]
      ,l.[LastModifiedByID] FROM tjc_hearing_log l left outer join users u on l.LastModifiedByID=u.userId 
	  WHERE (l.HearingDate BETWEEN @startDate AND @endDate) AND 
	  (Exists(Select j.JudgeID from tjc_hearing_jacs_judges j 
		Inner Join tjc_hearing_jacs_userid_by_userid x on j.JACSUserID = x.JACSUserID 
		WHERE x.UserID=@userId AND j.JudgeId=l.JudgeID and j.County=l.County) 
		OR l.LastModifiedByID = @userId )';
	
	DECLARE @ParameterDef nvarchar(MAX)
 
    SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @offset int, @pageSize int, @sortOrder nvarchar(20)';
							
	SET NOCOUNT ON;
	IF @status >=0
		BEGIN
			SET	@sql = @sql + ' AND Status =  @status';
		END
	
	SET	@sql = @sql + ' ORDER BY ' + @sortOrder + ' ' +@direction;
	SET	@sql = @sql + ' OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY';
	print @sql;
    -- Execute parameratized SQL
	EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @offset=@offset, @pageSize=@pageSize, @sortOrder=@sortOrder;
 
	
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_log_paged_chief_judge]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Parameratized Dynamic Sql for Mediation Case List Filtering>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_log_paged_chief_judge] 
	@userId int, @status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50), @judgeId int, @offset int, @pageSize int, @sortOrder nvarchar(20), @direction nvarchar(6)
AS
DECLARE @sql nvarchar(MAX)='';
DECLARE @ParameterDef nvarchar(MAX)
BEGIN
	IF ISNULL(@searchText,'') != '' AND @judgeId > 0
		BEGIN
			SET @searchText = '%' + @searchText + '%';
			SET @sql ='SELECT l.[LogID]
			  ,l.[CalendarID]
			  ,l.[County]
			  ,l.[OrderSigned]
			  ,l.[HearingDate]
			  ,l.[CaseName]
			  ,l.[CaseNumber]
			  ,l.[DIN]
			  ,l.[MotionTitle]
			  ,l.[DraftedBy]
			  ,l.[CourtNotes]
			  ,l.[DelayReason]
			  ,ISNULL(u.displayname,l.[JudgeID]) as JudgeID
			  ,l.[Status]
			  ,l.[CreatedByID]
			  ,l.[CreatedDate]
			  ,l.[LastModifiedDate]
			  ,l.[LastModifiedByID] FROM tjc_hearing_log l inner join users u on l.LastModifiedByID=u.userId 
			  WHERE (l.HearingDate BETWEEN @startDate AND @endDate) AND 
			  (casename like @searchText OR casenumber like @searchText OR din like @searchText 
			  OR motiontitle like @searchText OR draftedby like @searchText OR delayreason like @searchText 
			  OR courtnotes like @searchText) AND l.LastModifiedByID=@judgeId'; 

			SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50), @judgeId int, @offset int, @pageSize int, @sortOrder nvarchar(20)';
							
			SET NOCOUNT ON;
			IF @status >=0
				BEGIN
					SET	@sql = @sql + ' AND Status =  @status';
				END
	
			SET	@sql = @sql + ' ORDER BY ' + @sortOrder + ' ' +@direction;
			SET	@sql = @sql + ' OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY';
			print @sql;
			-- Execute parameratized SQL
			EXEC sp_Executesql  @sql, @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @searchText=@searchText, @judgeId=@judgeId, @offset=@offset, @pageSize=@pageSize, @sortOrder=@sortOrder;
		END
	ELSE IF ISNULL(@searchText,'') = '' AND @judgeId > 0
		BEGIN
			SET @sql = 'SELECT l.[LogID]
				,l.[CalendarID]
				,l.[County]
				,l.[OrderSigned]
				,l.[HearingDate]
				,l.[CaseName]
				,l.[CaseNumber]
				,l.[DIN]
				,l.[MotionTitle]
				,l.[DraftedBy]
				,l.[CourtNotes]
				,l.[DelayReason]
				,u.displayname
				,l.[Status]
				,l.[CreatedByID]
				,l.[CreatedDate]
				,l.[LastModifiedDate]
				,l.[LastModifiedByID] FROM tjc_hearing_log l inner join users u on l.LastModifiedByID=u.userId WHERE (l.HearingDate BETWEEN @startDate AND @endDate) AND l.LastModifiedByID=@judgeId'; 
			SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @judgeId int, @offset int, @pageSize int, @sortOrder nvarchar(20)';
			SET NOCOUNT ON;
			IF @status >=0
				BEGIN
					SET	@sql = @sql + ' AND Status =  @status';
				END
			SET	@sql = @sql + ' ORDER BY ' + @sortOrder + ' ' +@direction;
			SET	@sql = @sql + ' OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY';
			print @sql;
			-- Execute parameratized SQL
			EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @judgeId=@judgeId, @offset=@offset, @pageSize=@pageSize, @sortOrder=@sortOrder;
		END
	ELSE IF ISNULL(@searchText,'') != '' AND @judgeId = 0
		BEGIN
			SET @searchText = '%' + @searchText + '%';
			SET @sql ='SELECT l.[LogID]
			  ,l.[CalendarID]
			  ,l.[County]
			  ,l.[OrderSigned]
			  ,l.[HearingDate]
			  ,l.[CaseName]
			  ,l.[CaseNumber]
			  ,l.[DIN]
			  ,l.[MotionTitle]
			  ,l.[DraftedBy]
			  ,l.[CourtNotes]
			  ,l.[DelayReason]
			  ,ISNULL(u.displayname,l.[JudgeID]) as JudgeID
			  ,l.[Status]
			  ,l.[CreatedByID]
			  ,l.[CreatedDate]
			  ,l.[LastModifiedDate]
			  ,l.[LastModifiedByID] FROM tjc_hearing_log l inner join users u on l.LastModifiedByID=u.userId WHERE (l.HearingDate BETWEEN @startDate AND @endDate) AND (casename like @searchText OR casenumber like @searchText OR din like @searchText OR motiontitle like @searchText OR draftedby like @searchText OR delayreason like @searchText OR courtnotes like @searchText)'; 
			SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50), @offset int, @pageSize int, @sortOrder nvarchar(20)';
			SET NOCOUNT ON;
			IF @status >=0
				BEGIN
					SET	@sql = @sql + ' AND Status =  @status';
				END
			SET	@sql = @sql + ' ORDER BY ' + @sortOrder + ' ' +@direction;
			SET	@sql = @sql + ' OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY';
			print @sql;
			-- Execute parameratized SQL
			EXEC sp_Executesql  @sql, @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @searchText=@searchText, @offset=@offset, @pageSize=@pageSize, @sortOrder=@sortOrder;
		END
	ELSE 
		BEGIN
			SET @sql='SELECT l.[LogID]
				,l.[CalendarID]
				,l.[County]
				,l.[OrderSigned]
				,l.[HearingDate]
				,l.[CaseName]
				,l.[CaseNumber]
				,l.[DIN]
				,l.[MotionTitle]
				,l.[DraftedBy]
				,l.[CourtNotes]
				,l.[DelayReason]
				,ISNULL(u.displayname,l.[JudgeID]) as JudgeID
				,l.[Status]
				,l.[CreatedByID]
				,l.[CreatedDate]
				,l.[LastModifiedDate]
				,l.[LastModifiedByID] FROM tjc_hearing_log l inner join users u on l.LastModifiedByID=u.userId WHERE (l.HearingDate BETWEEN @startDate AND @endDate)';

			SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @offset int, @pageSize int, @sortOrder nvarchar(20)';						
			SET NOCOUNT ON;
			IF @status >=0
				BEGIN
					SET	@sql = @sql + ' AND Status =  @status';
				END
			SET	@sql = @sql + ' ORDER BY ' + @sortOrder + ' ' +@direction;
			SET	@sql = @sql + ' OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY';
			print @sql;
			-- Execute parameratized SQL
			EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @offset=@offset, @pageSize=@pageSize, @sortOrder=@sortOrder;
		END
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_log_paged_old]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Parameratized Dynamic Sql for Mediation Case List Filtering>
-- =============================================
Create PROCEDURE [dbo].[tjc_hearing_get_log_paged_old] 
	@userId int,@status int, @cutoffDate datetime,@offset int, @pageSize int,@sortOrder nvarchar(20),@direction nvarchar(6)
AS

BEGIN
	DECLARE @sql nvarchar(MAX)='SELECT * FROM tjc_hearing_log WHERE HearingDate >= @cutoffDate AND JudgeID in (Select j.JudgeID from tjc_hearing_jacs_judges j Inner Join tjc_hearing_jacs_userid_by_userid x on j.JACSUserID = x.JACSUserID WHERE x.UserID=@userId)';
	DECLARE @ParameterDef nvarchar(MAX)
 
    SET @ParameterDef = '@userId int, @status int, @cutoffDate datetime, @offset int, @pageSize int, @sortOrder nvarchar(20)';
							
	SET NOCOUNT ON;
	IF @status >=0
		BEGIN
			SET	@sql = @sql + ' AND Status =  @status';
		END
	
	SET	@sql = @sql + ' ORDER BY ' + @sortOrder + ' ' +@direction;
	SET	@sql = @sql + ' OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY';
	print @sql;
    -- Execute parameratized SQL
	EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId,@status=@status,@cutoffDate=@cutoffDate,@offset=@offset,@pageSize=@pageSize,@sortOrder=@sortOrder;
 
	
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_log_paged_search]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Parameratized Dynamic Sql for Mediation Case List Filtering>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_log_paged_search] 
	@userId int, @status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50), @offset int, @pageSize int, @sortOrder nvarchar(20), @direction nvarchar(6)
AS

BEGIN
	SET @searchText = '%' + @searchText + '%';
	DECLARE @sql nvarchar(MAX)='SELECT l.[LogID]
      ,l.[CalendarID]
      ,l.[County]
      ,l.[OrderSigned]
      ,l.[HearingDate]
      ,l.[CaseName]
      ,l.[CaseNumber]
      ,l.[DIN]
      ,l.[MotionTitle]
      ,l.[DraftedBy]
      ,l.[CourtNotes]
      ,l.[DelayReason]
      ,ISNULL(u.displayname,l.[JudgeID]) as JudgeID
      ,l.[Status]
      ,l.[CreatedByID]
      ,l.[CreatedDate]
      ,l.[LastModifiedDate]
      ,l.[LastModifiedByID] FROM tjc_hearing_log l left outer join users u on l.LastModifiedByID=u.userId WHERE ((l.HearingDate BETWEEN @startDate AND @endDate) AND (Exists(Select j.JudgeID from tjc_hearing_jacs_judges j Inner Join tjc_hearing_jacs_userid_by_userid x on j.JACSUserID = x.JACSUserID WHERE x.UserID=@userId AND j.JudgeId=l.JudgeID and j.County=l.County)  OR l.LastModifiedByID =@userId )) AND (casename like @searchText OR casenumber like @searchText OR din like @searchText OR motiontitle like @searchText OR draftedby like @searchText OR delayreason like @searchText OR courtnotes like @searchText)';
	
	DECLARE @ParameterDef nvarchar(MAX)
 
    SET @ParameterDef = '@userId int, @status int, @startDate datetime, @endDate datetime, @searchText nvarchar(50), @offset int, @pageSize int, @sortOrder nvarchar(20)';
							
	SET NOCOUNT ON;
	IF @status >=0
		BEGIN
			SET	@sql = @sql + ' AND Status =  @status';
		END
	
	SET	@sql = @sql + ' ORDER BY ' + @sortOrder + ' ' +@direction;
	SET	@sql = @sql + ' OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY';
	print @sql;
    -- Execute parameratized SQL
	EXEC sp_Executesql  @sql,  @ParameterDef, @userId=@userId, @status=@status, @startDate=@startDate, @endDate=@endDate, @searchText=@searchText, @offset=@offset, @pageSize=@pageSize, @sortOrder=@sortOrder;
 
	
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_user_jacs_judge_exists]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Deletes JACS Judge to UserID xref>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_user_jacs_judge_exists] 
	@jacsUserId int,@userId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   Select top 1 UserID from  tjc_hearing_jacs_userid_by_userid
   Where JACSUserID=@jacsUserId AND UserID<>@userId
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_get_user_jacs_judges]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/30/2024>
-- Description:	<Deletes JACS Judge to UserID xref>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_get_user_jacs_judges] 
	 @userId int,@county nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   Select hj.* from tjc_hearing_jacs_judges hj inner join tjc_hearing_jacs_userid_by_userid jx on hj.JacsUserID=jx.JacsUserID
   Where jx.UserID=@userId AND hj.County=@county;
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_import_app_hearings]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <7/31/2024>
-- Description:	<Imports JACS hearings into 60 day Log table>
-- =============================================
Create PROCEDURE [dbo].[tjc_hearing_import_app_hearings]
@startDate datetime,@endDate datetime, @userId int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	INSERT INTO tjc_hearing_log(HearingDate,CaseNumber,County,CaseName,CalendarID,JudgeID,Status,CreatedByID,CreatedDate,LastModifiedByID,LastModifiedDate)
	select caldate,casenum,'Sarasota',ISNULL(Plaintiff,'No Plaintiff') + ' v. ' + Isnull(Defendant,'No Defendant'),cal_id,JudgeID,0,1,GetDate(),NULL,GetDate() 
	from judsarsql03.jacssarasota.jacs.tbcourtcalendar c 
	where confirmnum is not null 
		and (caldate between @startDate and @endDate 
		and Not Exists (Select CalendarID From tjc_hearing_log l Where l.CalendarID= c.cal_id and l.county='Sarasota' )
		and Exists (Select JudgeID from tjc_hearing_jacs_userid_by_userid ref 
				inner join tjc_hearing_jacs_judges hj on ref.JACSUserID=hj.JacsUserID 
			Where hj.County='Sarasota' and hj.JudgeID=c.JudgeID and ref.UserID=@userId));

	INSERT INTO tjc_hearing_log(HearingDate,CaseNumber,County,CaseName,CalendarID,JudgeID,Status,CreatedByID,CreatedDate,LastModifiedByID,LastModifiedDate)
	select caldate,casenum,'Manatee',ISNULL(Plaintiff,'No Plaintiff') + ' v. ' + Isnull(Defendant,'No Defendant'),cal_id,JudgeID,0,1,GetDate(),NULL,GetDate() 
	from judsarsql03.jacsmanatee.jacs.tbcourtcalendar c 
	where confirmnum is not null 
		and (caldate between @startDate and @endDate 
		and Not Exists (Select CalendarID From tjc_hearing_log l Where l.CalendarID= c.cal_id and l.county='Manatee')
		and Exists (Select JudgeID from tjc_hearing_jacs_userid_by_userid ref 
				inner join tjc_hearing_jacs_judges hj on ref.JACSUserID=hj.JacsUserID 
			Where hj.County='Manatee' and hj.JudgeID=c.JudgeID and ref.UserID=@userId));

	INSERT INTO tjc_hearing_log(HearingDate,CaseNumber,County,CaseName,CalendarID,JudgeID,Status,CreatedByID,CreatedDate,LastModifiedByID,LastModifiedDate)
	select caldate,casenum,'DeSoto',ISNULL(Plaintiff,'No Plaintiff') + ' v. ' + Isnull(Defendant,'No Defendant'),cal_id,JudgeID,0,1,GetDate(),NULL,GetDate() 
	from judsarsql03.jacsdesoto.jacs.tbcourtcalendar c 
	where confirmnum is not null 
		and (caldate between @startDate and @endDate 
		and Not Exists (Select CalendarID From tjc_hearing_log l Where l.CalendarID= c.cal_id and l.county='DeSoto')
		and Exists (Select JudgeID from tjc_hearing_jacs_userid_by_userid ref 
				inner join tjc_hearing_jacs_judges hj on ref.JACSUserID=hj.JacsUserID 
			Where hj.County='DeSoto' and hj.JudgeID=c.JudgeID and ref.UserID=@userId));

   INSERT INTO tjc_hearing_log(HearingDate,CaseNumber,County,CaseName,CalendarID,JudgeID,Status,CreatedByID,CreatedDate,LastModifiedByID,LastModifiedDate)
   select HearingDate,CaseNumber,county,CaseName,CalendarID,CAST(JudgeID as nvarchar(20)),0,1,GetDate(),NULL,GetDate()
   from judsarsql03.jacsdesoto.dbo.tjc_hearing_clerk_export c where (hearingdate between @startDate and @endDate    
   and Not Exists (Select CalendarID From tjc_hearing_log l Where l.CalendarID= c.CalendarID and l.county= 'Benchmark')
   and Exists (Select JudgeID from tjc_hearing_jacs_userid_by_userid ref 
				inner join tjc_hearing_jacs_judges hj on ref.JACSUserID=hj.JacsUserID 
			Where hj.County='Benchmark' and hj.JudgeID=c.JudgeID and ref.UserID=@userId));
          
   INSERT INTO tjc_hearing_log(HearingDate,CaseNumber,County,CaseName,CalendarID,JudgeID,Status,CreatedByID,CreatedDate,LastModifiedByID,LastModifiedDate)
   select HearingDate,CaseNumber,county,CaseName,CalendarID,CAST(JudgeID as nvarchar(20)),0,1,GetDate(),NULL,GetDate()
   from judsarsql03.jacsdesoto.dbo.tjc_hearing_clerk_export c where (hearingdate between @startDate and @endDate    
   and Not Exists (Select CalendarID From tjc_hearing_log l Where l.CalendarID= c.CalendarID and l.county= 'Clericus')
   and Exists (Select JudgeID from tjc_hearing_jacs_userid_by_userid ref 
				inner join tjc_hearing_jacs_judges hj on ref.JACSUserID=hj.JacsUserID 
			Where hj.County='Clericus' and hj.JudgeID=c.JudgeID and ref.UserID=@userId));

END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_import_court_counsel_judges]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Joe Terhune>
-- Create date: <8/19/2024>
-- Description:	<Imports Court counsel Judges>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_import_court_counsel_judges] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   INSERT INTO [dbo].[tjc_hearing_jacs_judges]
           ([JudgeID]
           ,[County]
           ,[JudgeName])
    Select JudgeID,'CourtCounsel',JudgeName
	From tjc_hearing_cc_judges jcc
	Where NOT EXISTS(select judgeID from tjc_hearing_jacs_judges j where j.judgeID=jcc.judgeID and j.County='CourtCounsel')
END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_import_jacs_hearings]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[tjc_hearing_import_jacs_hearings]
	@userId int,@startDate datetime, @endDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	INSERT INTO tjc_hearing_log(HearingDate,CaseNumber,County,CaseName,CalendarID,JudgeID,Status,CreatedByID,CreatedDate,LastModifiedByID,LastModifiedDate)
   select caldate,casenum,'Sarasota',ISNULL(Plaintiff,'No Plaintiff') + ' v. ' + Isnull(Defendant,'No Defendant'),cal_id,JudgeID,0,1,GetDate(),NULL,GetDate() 
   from judsarsql03.jacssarasota.jacs.tbcourtcalendar c where confirmnum is not null and (caldate between @startDate and @endDate) and Not Exists (Select CalendarID From tjc_hearing_log l Where l.CalendarID= c.cal_id and l.county='Sarasota') 
	AND Exists(select jj.JudgeID from tjc_hearing_jacs_judges jj inner join tjc_hearing_jacs_userid_by_userid ref on jj.JacsUserID=ref.JACSUserID Where ref.UserID=@userId and jj.County='Sarasota' and jj.JudgeID=c.JudgeID)

   INSERT INTO tjc_hearing_log(HearingDate,CaseNumber,County,CaseName,CalendarID,JudgeID,Status,CreatedByID,CreatedDate,LastModifiedByID,LastModifiedDate)
   select caldate,casenum,'Manatee',ISNULL(Plaintiff,'No Plaintiff') + ' v. ' + Isnull(Defendant,'No Defendant'),cal_id,JudgeID,0,1,GetDate(),NULL,GetDate()
   from judsarsql03.jacsmanatee.jacs.tbcourtcalendar c where confirmnum is not null and (caldate between @startDate and @endDate) and Not Exists (Select CalendarID From tjc_hearing_log l Where l.CalendarID= c.cal_id and l.county='Manatee')
   	AND Exists(select jj.JudgeID from tjc_hearing_jacs_judges jj inner join tjc_hearing_jacs_userid_by_userid ref on jj.JacsUserID=ref.JACSUserID Where ref.UserID=@userId and jj.County='Manatee' and jj.JudgeID=c.JudgeID)


     INSERT INTO tjc_hearing_log(HearingDate,CaseNumber,County,CaseName,CalendarID,JudgeID,Status,CreatedByID,CreatedDate,LastModifiedByID,LastModifiedDate)
   select caldate,casenum,'DeSoto',ISNULL(Plaintiff,'No Plaintiff') + ' v. ' + Isnull(Defendant,'No Defendant'),cal_id,JudgeID,0,1,GetDate(),NULL,GetDate()
   from judsarsql03.jacsdesoto.jacs.tbcourtcalendar c where confirmnum is not null and (caldate between @startDate and @endDate) and Not Exists (Select CalendarID From tjc_hearing_log l Where l.CalendarID= c.cal_id and l.county='DeSoto')
   	AND Exists(select jj.JudgeID from tjc_hearing_jacs_judges jj inner join tjc_hearing_jacs_userid_by_userid ref on jj.JacsUserID=ref.JACSUserID Where ref.UserID=@userId and jj.County='DeSoto' and jj.JudgeID=c.JudgeID)

END
GO
/****** Object:  StoredProcedure [dbo].[tjc_hearing_list_judge_ja_ref]    Script Date: 9/9/2025 1:56:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[tjc_hearing_list_judge_ja_ref] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select * FROM tjc_hearing_judge_ja  
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[22] 4[42] 2[17] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "h"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 212
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "jj"
            Begin Extent = 
               Top = 6
               Left = 250
               Bottom = 136
               Right = 420
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ref"
            Begin Extent = 
               Top = 6
               Left = 458
               Bottom = 102
               Right = 628
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2565
         Alias = 2355
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'tjc_hearing_cc'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'tjc_hearing_cc'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "aws_cc_Requestor (intranet.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 119
               Right = 212
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'tjc_hearing_cc_judges'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'tjc_hearing_cc_judges'
GO
