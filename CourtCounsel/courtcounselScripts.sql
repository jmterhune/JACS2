USE [intranet]
GO

/****** Object:  StoredProcedure [dbo].[aws_cc_AddActionTaken]    Script Date: 4/15/2026 9:53:28 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_AddActionTaken]
	@Action varchar(35)
AS

INSERT INTO aws_cc_ActionTaken (
	[Action]
) VALUES (
	@Action
)

select SCOPE_IDENTITY()

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_AddAttorney]    Script Date: 4/15/2026 9:53:28 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_AddAttorney]
	@AttorneyName varchar(25),
	@IsActive bit
AS

INSERT INTO aws_cc_Attorney (
	[AttorneyName],
	[IsActive]
) VALUES (
	@AttorneyName,
	@IsActive
)

select SCOPE_IDENTITY()

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_AddCaseType]    Script Date: 4/15/2026 9:53:28 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_AddCaseType]
	@CaseType varchar(50)
AS

INSERT INTO aws_cc_CaseType (
	[CaseType]
) VALUES (
	@CaseType
)

select SCOPE_IDENTITY()

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_AddCounty]    Script Date: 4/15/2026 9:53:28 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_AddCounty]
	@County varchar(50)
AS

INSERT INTO aws_cc_County (
	[County]
) VALUES (
	@County
)

select SCOPE_IDENTITY()

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_AddHistory]    Script Date: 4/15/2026 9:53:28 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[aws_cc_AddHistory]
	@DateReceived smalldatetime,
	@CaseNumber varchar(18),
	@PartyName varchar(100),
	@CaseType varchar(50),
	@DateDue smalldatetime,
	@RequestedBy varchar(25),
	@Responsible varchar(25),
	@County varchar(10),
	@Description varchar(100),
	@Phase varchar(25),
	@Action varchar(35),
	@FollowUp varchar(3),
	@DateCompleted smalldatetime,
	@TimeSpent varchar(20),
	@Comments varchar(8000),
	@statusName nvarchar(50),
	@motionFiled smalldatetime
AS

INSERT INTO aws_cc_History (
	[DateReceived],
	[CaseNumber],
	[PartyName],
	[CaseType],
	[DateDue],
	[RequestedBy],
	[Responsible],
	[County],
	[Description],
	[Phase],
	[Action],
	[FollowUp],
	[DateCompleted],
	[TimeSpent],
	[Comments],
	[StatusName],
	[MotionFiled],
	[LastModifiedDate]
) VALUES (
	@DateReceived,
	@CaseNumber,
	@PartyName,
	@CaseType,
	@DateDue,
	@RequestedBy,
	@Responsible,
	@County,
	@Description,
	@Phase,
	@Action,
	@FollowUp,
	@DateCompleted,
	@TimeSpent,
	@Comments,
	@statusName,
	@motionFiled,
	GetDate()
)

select SCOPE_IDENTITY()


GO

/****** Object:  StoredProcedure [dbo].[aws_cc_AddPhase]    Script Date: 4/15/2026 9:53:28 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_AddPhase]
	@Phase varchar(25)
AS

INSERT INTO aws_cc_Phase (
	[Phase]
) VALUES (
	@Phase
)

select SCOPE_IDENTITY()

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_AddRequestor]    Script Date: 4/15/2026 9:53:28 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_AddRequestor]
	@RequestorName varchar(25),
	@IsActive bit
AS

INSERT INTO aws_cc_Requestor (
	[RequestorName],
	[IsActive]
) VALUES (
	@RequestorName,
	@IsActive
)

select SCOPE_IDENTITY()

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_AddTimeSpent]    Script Date: 4/15/2026 9:53:28 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_AddTimeSpent]
	@timeSpan varchar(50),
	@IsActive bit
AS

INSERT INTO aws_cc_TimeSpent (
	[timeSpan],
	[IsActive]
) VALUES (
	@timeSpan,
	@IsActive
)

select SCOPE_IDENTITY()

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_DeleteActionTaken]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_DeleteActionTaken]
	@ActionId int
AS

DELETE FROM aws_cc_ActionTaken
WHERE
	[ActionId] = @ActionId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_DeleteAttorney]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_DeleteAttorney]
	@AttorneyId int
AS

DELETE FROM aws_cc_Attorney
WHERE
	[AttorneyId] = @AttorneyId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_DeleteCaseType]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_DeleteCaseType]
	@caseTypeId int
AS

DELETE FROM aws_cc_CaseType
WHERE
	[caseTypeId] = @caseTypeId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_DeleteCounty]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_DeleteCounty]
	@countyId int
AS

DELETE FROM aws_cc_County
WHERE
	[countyId] = @countyId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_DeleteHistory]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_DeleteHistory]
	@logId int
AS

DELETE FROM aws_cc_History
WHERE
	[logId] = @logId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_DeletePhase]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_DeletePhase]
	@phaseId int
AS

DELETE FROM aws_cc_Phase
WHERE
	[phaseId] = @phaseId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_DeleteRequestor]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_DeleteRequestor]
	@RequestorId int
AS

DELETE FROM aws_cc_Requestor
WHERE
	[RequestorId] = @RequestorId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_DeleteTimeSpent]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_DeleteTimeSpent]
	@timeSpanId int
AS

DELETE FROM aws_cc_TimeSpent
WHERE
	[timeSpanId] = @timeSpanId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetActionTaken]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[aws_cc_GetActionTaken]
	@ActionId int
	
AS

SELECT
	[ActionId],
	[Action]
FROM aws_cc_ActionTaken
WHERE
	[ActionId] = @ActionId
	

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetAttorney]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[aws_cc_GetAttorney]
	@AttorneyId int
	
AS

SELECT
	[AttorneyId],
	[AttorneyName],
	[IsActive]
FROM aws_cc_Attorney
WHERE
	[AttorneyId] = @AttorneyId
	

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetCaseNamesByCaseNumber]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO


create PROCEDURE [dbo].[aws_cc_GetCaseNamesByCaseNumber]
	@caseNumber nvarchar(50)
	
AS

SELECT
	
	[CaseNumber],
	[PartyName]
FROM aws_cc_History
WHERE
	CaseNumber=@caseNumber
	
GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetCaseType]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[aws_cc_GetCaseType]
	@caseTypeId int
	
AS

SELECT
	[caseTypeId],
	[CaseType]
FROM aws_cc_CaseType
WHERE
	[caseTypeId] = @caseTypeId
	

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetCounty]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[aws_cc_GetCounty]
	@countyId int
	
AS

SELECT
	[countyId],
	[County]
FROM aws_cc_County
WHERE
	[countyId] = @countyId
	

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetHistory]    Script Date: 4/15/2026 9:53:29 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO




CREATE PROCEDURE [dbo].[aws_cc_GetHistory]
	@logId int
	
AS

SELECT *
FROM aws_cc_History
WHERE
	[logId] = @logId
	


GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetHistoryCountReport]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[aws_cc_GetHistoryCountReport] 
      -- Add the parameters for the stored procedure here
      @StartDate datetime,
      @EndDate datetime,
      @Status char(1),
      @Attorney varchar(50) = NULL,
      @County varchar(50) = NULL,
      @Requestor varchar(50) = NULL
AS
BEGIN
      SET NOCOUNT ON;
      IF @Attorney = 'all' SET @Attorney = NULL
      IF @County = 'all'   SET @County = NULL
      IF @Requestor = 'all' SET @Requestor = NULL

      IF    @Status='R'
            Select ct.caseType,ISNULL(history.casecount,0) as casecount FROM dbo.aws_cc_CaseType ct Left Outer Join (SELECT casetype, COUNT(logid) as casecount From dbo.aws_cc_History 
WHERE (DateReceived BETWEEN @StartDate AND  @EndDate) AND County = ISNULL(@County,County) AND requestedBy = ISNULL(@Requestor,requestedBy) AND Responsible = ISNULL(@Attorney,Responsible)
            GROUP BY casetype) history ON history.CaseType = ct.CaseType
             ORDER BY ct.casetype
      ELSE
            Select ct.casetype,ISNULL(history.casecount,0) as casecount FROM dbo.aws_cc_CaseType ct Left Outer Join (SELECT casetype,COUNT(logid) as casecount From dbo.aws_cc_History 
WHERE (DateCompleted BETWEEN @StartDate AND @EndDate) AND County = ISNULL(@County,County) AND requestedBy = ISNULL(@Requestor,requestedBy) AND Responsible = ISNULL(@Attorney,Responsible)
            GROUP BY casetype) history ON history.CaseType = ct.CaseType
            ORDER BY ct.casetype
END
GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetPhase]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[aws_cc_GetPhase]
	@phaseId int
	
AS

SELECT
	[phaseId],
	[Phase]
FROM aws_cc_Phase
WHERE
	[phaseId] = @phaseId
	

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetRequestor]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[aws_cc_GetRequestor]
	@RequestorId int
	
AS

SELECT
	[RequestorId],
	[RequestorName],
	[IsActive]
FROM aws_cc_Requestor
WHERE
	[RequestorId] = @RequestorId
	

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_GetTimeSpent]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[aws_cc_GetTimeSpent]
	@timeSpanId int
	
AS

SELECT
	[timeSpanId],
	[timeSpan],
	[IsActive]
FROM aws_cc_TimeSpent
WHERE
	[timeSpanId] = @timeSpanId
	

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListActionsTaken]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_ListActionsTaken]
AS

SELECT
	[ActionId],
	[Action]
FROM aws_cc_ActionTaken
Order By action

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListActiveAttorneys]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



Create PROCEDURE [dbo].[aws_cc_ListActiveAttorneys]
AS

SELECT
	[AttorneyId],
	[AttorneyName]
	FROM aws_cc_Attorney
WHERE isactive=1
Order by [AttorneyName]

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListActiveRequestors]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



create PROCEDURE [dbo].[aws_cc_ListActiveRequestors]
AS

SELECT
	[RequestorId],
	[RequestorName],
	[IsActive]
FROM aws_cc_Requestor
Where IsActive=1
Order by RequestorName

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListActiveTimeSpents]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



create PROCEDURE [dbo].[aws_cc_ListActiveTimeSpents]
AS

SELECT
	[timeSpanId],
	[timeSpan],
	[IsActive]
FROM aws_cc_TimeSpent
WHERE IsActive=1
Order by timeSpan

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListAttorneys]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_ListAttorneys]
AS

SELECT
	[AttorneyId],
	[AttorneyName],
	[IsActive]
FROM aws_cc_Attorney
Order by [AttorneyName]

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListCaseNumbersByAttorney]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[aws_cc_ListCaseNumbersByAttorney]
	@Attorney varchar(200),@status varchar(3)
AS
BEGIN
	SET NOCOUNT ON;
	IF @status='A'
		BEGIN
			SELECT DISTINCT h.partyname, responsible,h.StatusName,
				h.DateReceived, h.DateCompleted, h.CaseType, h.CaseNumber,0 as currentStatus
			FROM aws_cc_History h 
			WHERE h.responsible LIKE @Attorney AND h.DateCompleted IS NULL AND DateReceived <= GetDate()
			ORDER BY h.DateReceived,h.PartyName
		END
    IF @status='I'
		BEGIN
			SELECT DISTINCT h.partyname, responsible, h.StatusName,
				h.DateReceived, h.DateCompleted, h.CaseType, h.CaseNumber,1 as currentStatus
			FROM aws_cc_History h 
			WHERE h.responsible LIKE @Attorney AND h.DateCompleted IS NULL AND DateReceived > GetDate()
			ORDER BY h.DateReceived,h.PartyName

		END
	IF @status='C'
		BEGIN
			SELECT DISTINCT h.partyname, responsible, h.StatusName,
				h.DateReceived, h.DateCompleted, h.CaseType, h.CaseNumber,2 as currentStatus
			FROM aws_cc_History h 
			WHERE h.responsible LIKE @Attorney AND h.DateCompleted IS NOT NULL 
			ORDER BY h.DateReceived,h.PartyName

		END

	IF @status='AIC'
		BEGIN
			SELECT DISTINCT h.partyname, responsible, h.StatusName,
				h.DateReceived, h.DateCompleted, h.CaseType, h.CaseNumber,dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted) AS CurrentStatus
			FROM aws_cc_History h 
			WHERE h.responsible LIKE @Attorney
			ORDER BY dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted),h.DateReceived,h.PartyName

		END
	IF @status='AI'
		BEGIN
			SELECT DISTINCT h.partyname, responsible, h.StatusName,
				h.DateReceived, h.DateCompleted, h.CaseType, h.CaseNumber,dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted) AS CurrentStatus
			FROM aws_cc_History h 
			WHERE h.responsible LIKE @Attorney and h.DateCompleted IS NULL
			ORDER BY  dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted),h.DateReceived,h.PartyName

		END
	IF @status='AC'
		BEGIN
			SELECT DISTINCT h.partyname, responsible, h.StatusName,
				h.DateReceived, h.DateCompleted, h.CaseType, h.CaseNumber,dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted) AS CurrentStatus
			FROM aws_cc_History h 
			WHERE h.responsible LIKE @Attorney AND (DateReceived <= GetDate() OR h.DateCompleted IS NOT NULL)
			ORDER BY  dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted),h.DateReceived,h.PartyName

		END
   IF @status='IC'
		BEGIN
			SELECT DISTINCT h.partyname, responsible, h.StatusName,
				h.DateReceived, h.DateCompleted, h.CaseType, h.CaseNumber,dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted) AS CurrentStatus
			FROM aws_cc_History h 
			WHERE h.responsible LIKE @Attorney AND (DateReceived > GetDate() OR h.DateCompleted IS NOT NULL)
			ORDER BY  dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted),h.DateReceived,h.PartyName

		END
	IF @status='all'
		BEGIN
			SELECT DISTINCT h.partyname, responsible, h.StatusName,
				h.DateReceived, h.DateCompleted, h.CaseType, h.CaseNumber,dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted) AS CurrentStatus
			FROM aws_cc_History h 
			WHERE h.responsible LIKE @Attorney
			ORDER BY  dbo.aws_cc_GetActionStatus(h.DateReceived,h.DateCompleted),h.DateReceived,h.PartyName

		END

END

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListCaseNumbersByCaseName]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[aws_cc_ListCaseNumbersByCaseName]
	@PartyName varchar(200)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT DISTINCT    h.Responsible, h.CaseType, h.CaseNumber, h.PartyName
	FROM         aws_cc_Attorney a INNER JOIN
			  aws_cc_History h ON a.AttorneyName = h.Responsible
	WHERE h.PartyName Like '%'+@PartyName+'%'
	ORDER BY h.PartyName
END

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListCaseTypes]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_ListCaseTypes]
AS

SELECT
	[caseTypeId],
	[CaseType]
FROM aws_cc_CaseType
Order By CaseType

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListCountys]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_ListCountys]
AS

SELECT
	[countyId],
	[County]
FROM aws_cc_County
order by county

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListHistorys]    Script Date: 4/15/2026 9:53:30 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[aws_cc_ListHistorys]
AS

SELECT *
FROM aws_cc_History


GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListLogByCaseNumber]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[aws_cc_ListLogByCaseNumber]
@caseNumber varchar(100),@casename varchar(100)
AS

IF @casename='' OR @casename IS NULL
	BEGIN
		SELECT *
		FROM aws_cc_History
		WHERE CaseNumber LIKE @caseNumber
		ORDER BY dbo.aws_cc_GetActionStatus(DateReceived,DateCompleted),StatusName
	END
ELSE
	BEGIN
		SELECT *
		FROM aws_cc_History
		WHERE CaseNumber LIKE @caseNumber and PartyName like @casename
		ORDER BY dbo.aws_cc_GetActionStatus(DateReceived,DateCompleted),StatusName
	END

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListOverDueHistory]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_ListOverDueHistory]
@cutoffDate datetime
AS

SELECT *
FROM aws_cc_History
WHERE DateReceived<=@cutoffDate AND DateCompleted IS NULL

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListPartyNamesByCaseNumber]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [dbo].[aws_cc_ListPartyNamesByCaseNumber]
@caseNumber varchar(100),@casename varchar(100)
AS

IF @casename='' OR @casename IS NULL
	BEGIN
		SELECT DISTINCT PartyName
		FROM aws_cc_History
		WHERE CaseNumber LIKE @caseNumber
		ORDER BY PartyName
	END
ELSE
	BEGIN
		SELECT DISTINCT PartyName
		FROM aws_cc_History
		WHERE CaseNumber LIKE @caseNumber and PartyName LIKE @casename
		ORDER BY PartyName
	END
GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListPhases]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_ListPhases]
AS

SELECT
	[phaseId],
	[Phase]
FROM aws_cc_Phase

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListRequestors]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_ListRequestors]
AS

SELECT
	[RequestorId],
	[RequestorName],
	[IsActive]
FROM aws_cc_Requestor
order by RequestorName

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_ListTimeSpents]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_ListTimeSpents]
AS

SELECT
	[timeSpanId],
	[timeSpan],
	[IsActive]
FROM aws_cc_TimeSpent

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdateActionTaken]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_UpdateActionTaken]
	@ActionId int, 
	@Action varchar(35) 
AS

UPDATE aws_cc_ActionTaken SET
	[Action] = @Action
WHERE
	[ActionId] = @ActionId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdateAttorney]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_UpdateAttorney]
	@AttorneyId int, 
	@AttorneyName varchar(25), 
	@IsActive bit 
AS

UPDATE aws_cc_Attorney SET
	[AttorneyName] = @AttorneyName,
	[IsActive] = @IsActive
WHERE
	[AttorneyId] = @AttorneyId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdateCaseName]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO




create PROCEDURE [dbo].[aws_cc_UpdateCaseName]
	@CaseNumber varchar(18), 
	@PartyName varchar(100) 
AS

UPDATE aws_cc_History SET
	[PartyName] = @PartyName
WHERE
	CaseNumber = @CaseNumber


GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdateCaseType]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_UpdateCaseType]
	@caseTypeId int, 
	@CaseType varchar(50) 
AS

UPDATE aws_cc_CaseType SET
	[CaseType] = @CaseType
WHERE
	[caseTypeId] = @caseTypeId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdateCounty]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_UpdateCounty]
	@countyId int, 
	@County varchar(50) 
AS

UPDATE aws_cc_County SET
	[County] = @County
WHERE
	[countyId] = @countyId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdateHistory]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[aws_cc_UpdateHistory]
	@logId int, 
	@DateReceived smalldatetime, 
	@CaseNumber varchar(18), 
	@PartyName varchar(100), 
	@CaseType varchar(50), 
	@DateDue smalldatetime, 
	@RequestedBy varchar(25), 
	@Responsible varchar(25), 
	@County varchar(10), 
	@Description varchar(100), 
	@Phase varchar(25), 
	@Action varchar(35), 
	@FollowUp varchar(3), 
	@DateCompleted smalldatetime, 
	@TimeSpent varchar(20), 
	@Comments varchar(8000),
	@statusName nvarchar(50),
	@motionfiled smalldatetime 
AS

UPDATE aws_cc_History SET
	[DateReceived] = @DateReceived,
	[CaseNumber] = @CaseNumber,
	[PartyName] = @PartyName,
	[CaseType] = @CaseType,
	[DateDue] = @DateDue,
	[RequestedBy] = @RequestedBy,
	[Responsible] = @Responsible,
	[County] = @County,
	[Description] = @Description,
	[Phase] = @Phase,
	[Action] = @Action,
	[FollowUp] = @FollowUp,
	[DateCompleted] = @DateCompleted,
	[TimeSpent] = @TimeSpent,
	[Comments] = @Comments,
	[StatusName] = @statusName,
	[MotionFiled]= @motionfiled,
	[LastModifiedDate]=GetDate()
WHERE
	[logId] = @logId


GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdatePhase]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_UpdatePhase]
	@phaseId int, 
	@Phase varchar(25) 
AS

UPDATE aws_cc_Phase SET
	[Phase] = @Phase
WHERE
	[phaseId] = @phaseId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdateRequestor]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_UpdateRequestor]
	@RequestorId int, 
	@RequestorName varchar(25), 
	@IsActive bit 
AS

UPDATE aws_cc_Requestor SET
	[RequestorName] = @RequestorName,
	[IsActive] = @IsActive
WHERE
	[RequestorId] = @RequestorId

GO

/****** Object:  StoredProcedure [dbo].[aws_cc_UpdateTimeSpent]    Script Date: 4/15/2026 9:53:31 AM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aws_cc_UpdateTimeSpent]
	@timeSpanId int, 
	@timeSpan varchar(50), 
	@IsActive bit 
AS

UPDATE aws_cc_TimeSpent SET
	[timeSpan] = @timeSpan,
	[IsActive] = @IsActive
WHERE
	[timeSpanId] = @timeSpanId

GO

