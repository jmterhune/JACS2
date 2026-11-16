/*  Copies data from the ORIGINAL tjc_med_* tables into the NEW tjc_modules_med_* tables.
    Run it manually, once, against the intranet database. It never modifies the old tables.

    - Reference data (regions, groups, case types, appearances, issues, stages of action,
      mediators, attorneys and the group/lookup link tables) is always copied, keeping the
      same IDs, so the new module is usable immediately.
    - Historical case data (cases, sessions, events and their links) is copied only when
      @IncludeHistory = 1. Leave it 0 to start the new module's statistics from empty,
      for example so the revised fee tiers are not mixed with old sessions.
    - Each table is copied only if the NEW table is empty, so re-running is safe. */

SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @IncludeHistory bit = 0;  -- set to 1 to also copy cases, sessions and events

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_appearances)
BEGIN
  SET IDENTITY_INSERT dbo.tjc_modules_med_appearances ON;
  INSERT INTO dbo.tjc_modules_med_appearances ([AppearanceId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [AppearanceId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_appearances;
  PRINT 'tjc_modules_med_appearances: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  SET IDENTITY_INSERT dbo.tjc_modules_med_appearances OFF;
END
ELSE PRINT 'tjc_modules_med_appearances: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_attorneys)
BEGIN
  SET IDENTITY_INSERT dbo.tjc_modules_med_attorneys ON;
  INSERT INTO dbo.tjc_modules_med_attorneys ([AttorneyId], [FirstName], [LastName], [Firm], [Email], [Phone], [Extension], [Address], [City], [State], [Zip], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [AttorneyId], [FirstName], [LastName], [Firm], [Email], [Phone], [Extension], [Address], [City], [State], [Zip], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_attorneys;
  PRINT 'tjc_modules_med_attorneys: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  SET IDENTITY_INSERT dbo.tjc_modules_med_attorneys OFF;
END
ELSE PRINT 'tjc_modules_med_attorneys: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_casetypes)
BEGIN
  SET IDENTITY_INSERT dbo.tjc_modules_med_casetypes ON;
  INSERT INTO dbo.tjc_modules_med_casetypes ([CaseTypeId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [CaseTypeId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_casetypes;
  PRINT 'tjc_modules_med_casetypes: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  SET IDENTITY_INSERT dbo.tjc_modules_med_casetypes OFF;
END
ELSE PRINT 'tjc_modules_med_casetypes: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_groups)
BEGIN
  SET IDENTITY_INSERT dbo.tjc_modules_med_groups ON;
  INSERT INTO dbo.tjc_modules_med_groups ([GroupId], [Description], [CourtOrdered], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [GroupId], [Description], [CourtOrdered], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_groups;
  PRINT 'tjc_modules_med_groups: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  SET IDENTITY_INSERT dbo.tjc_modules_med_groups OFF;
END
ELSE PRINT 'tjc_modules_med_groups: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_issues)
BEGIN
  SET IDENTITY_INSERT dbo.tjc_modules_med_issues ON;
  INSERT INTO dbo.tjc_modules_med_issues ([IssueId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [IssueId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_issues;
  PRINT 'tjc_modules_med_issues: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  SET IDENTITY_INSERT dbo.tjc_modules_med_issues OFF;
END
ELSE PRINT 'tjc_modules_med_issues: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_mediators)
BEGIN
  SET IDENTITY_INSERT dbo.tjc_modules_med_mediators ON;
  INSERT INTO dbo.tjc_modules_med_mediators ([MediatorId], [LastName], [FirstName], [Email], [Phone], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [MediatorId], [LastName], [FirstName], [Email], [Phone], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_mediators;
  PRINT 'tjc_modules_med_mediators: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  SET IDENTITY_INSERT dbo.tjc_modules_med_mediators OFF;
END
ELSE PRINT 'tjc_modules_med_mediators: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_regions)
BEGIN
  SET IDENTITY_INSERT dbo.tjc_modules_med_regions ON;
  INSERT INTO dbo.tjc_modules_med_regions ([RegionId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [RegionId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_regions;
  PRINT 'tjc_modules_med_regions: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  SET IDENTITY_INSERT dbo.tjc_modules_med_regions OFF;
END
ELSE PRINT 'tjc_modules_med_regions: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_stage_action)
BEGIN
  SET IDENTITY_INSERT dbo.tjc_modules_med_stage_action ON;
  INSERT INTO dbo.tjc_modules_med_stage_action ([StageOfActionId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [StageOfActionId], [Description], [Active], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_stage_action;
  PRINT 'tjc_modules_med_stage_action: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  SET IDENTITY_INSERT dbo.tjc_modules_med_stage_action OFF;
END
ELSE PRINT 'tjc_modules_med_stage_action: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_appearance_by_group)
BEGIN
  INSERT INTO dbo.tjc_modules_med_appearance_by_group ([AppearanceId], [GroupId], [SortOrder], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [AppearanceId], [GroupId], [SortOrder], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_appearance_by_group;
  PRINT 'tjc_modules_med_appearance_by_group: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
END
ELSE PRINT 'tjc_modules_med_appearance_by_group: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_casetype_by_group)
BEGIN
  INSERT INTO dbo.tjc_modules_med_casetype_by_group ([GroupId], [CaseTypeId], [SortOrder], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [GroupId], [CaseTypeId], [SortOrder], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_casetype_by_group;
  PRINT 'tjc_modules_med_casetype_by_group: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
END
ELSE PRINT 'tjc_modules_med_casetype_by_group: skipped (already has data)';

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_issue_by_group)
BEGIN
  INSERT INTO dbo.tjc_modules_med_issue_by_group ([GroupId], [IssueId], [SortOrder], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
  SELECT [GroupId], [IssueId], [SortOrder], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_issue_by_group;
  PRINT 'tjc_modules_med_issue_by_group: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
END
ELSE PRINT 'tjc_modules_med_issue_by_group: skipped (already has data)';

IF @IncludeHistory = 1
BEGIN
  IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_cases)
  BEGIN
    SET IDENTITY_INSERT dbo.tjc_modules_med_cases ON;
    INSERT INTO dbo.tjc_modules_med_cases ([CaseId], [RegionId], [GroupId], [CaseNumber], [CDSPNumber], [p1_FirstName], [p1_LastName], [p1_business], [p2_FirstName], [p2_LastName], [p2_business], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
    SELECT [CaseId], [RegionId], [GroupId], [CaseNumber], [CDSPNumber], [p1_FirstName], [p1_LastName], [p1_business], [p2_FirstName], [p2_LastName], [p2_business], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_cases;
    PRINT 'tjc_modules_med_cases: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
    SET IDENTITY_INSERT dbo.tjc_modules_med_cases OFF;
  END
  ELSE PRINT 'tjc_modules_med_cases: skipped (already has data)';

  IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_event_appearances)
  BEGIN
    INSERT INTO dbo.tjc_modules_med_event_appearances ([EventId], [AppearanceId], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
    SELECT [EventId], [AppearanceId], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_event_appearances;
    PRINT 'tjc_modules_med_event_appearances: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  END
  ELSE PRINT 'tjc_modules_med_event_appearances: skipped (already has data)';

  IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_sessions)
  BEGIN
    SET IDENTITY_INSERT dbo.tjc_modules_med_sessions ON;
    INSERT INTO dbo.tjc_modules_med_sessions ([SessionId], [CaseId], [PrimaryCaseType], [StageOfAction], [p1_AttorneyId], [p1_ProSe], [p1_FeesPaid], [p1_FeesOwed], [p2_AttorneyId], [p2_ProSe], [p2_FeesPaid], [p2_FeesOwed], [p1_FTA], [p2_FTA], [MediationDate], [ReferralDate], [ProgramReferralSource], [ArbitrationReferral], [CircuitCivilReferral], [FeeAmount], [FeeJudgement], [FeeAgreement], [PTC_CourtOrdered], [ChildrenInvolved], [ParentsInvolved], [HeldByPhone], [Comment], [OTS], [FeeWaiver], [Inmate], [Interpreter], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById], [Mediator])
    SELECT [SessionId], [CaseId], [PrimaryCaseType], [StageOfAction], [p1_AttorneyId], [p1_ProSe], [p1_FeesPaid], [p1_FeesOwed], [p2_AttorneyId], [p2_ProSe], [p2_FeesPaid], [p2_FeesOwed], [p1_FTA], [p2_FTA], [MediationDate], [ReferralDate], [ProgramReferralSource], [ArbitrationReferral], [CircuitCivilReferral], [FeeAmount], [FeeJudgement], [FeeAgreement], [PTC_CourtOrdered], [ChildrenInvolved], [ParentsInvolved], [HeldByPhone], [Comment], [OTS], [FeeWaiver], [Inmate], [Interpreter], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById], [Mediator] FROM dbo.tjc_med_sessions;
    PRINT 'tjc_modules_med_sessions: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
    SET IDENTITY_INSERT dbo.tjc_modules_med_sessions OFF;
  END
  ELSE PRINT 'tjc_modules_med_sessions: skipped (already has data)';

  IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_events)
  BEGIN
    SET IDENTITY_INSERT dbo.tjc_modules_med_events ON;
    INSERT INTO dbo.tjc_modules_med_events ([EventId], [SessionId], [MediationHeld], [ReasonNotHeld], [EventDate], [MediatorId], [MediatorType], [Comments], [AgreementType], [AgreementSubmittedParties], [AgreementPreparedAttorney], [AgreementSigned], [AdjournedTimeRemaining], [TimeRemaining], [SignedCount1], [SignedCount2], [SignedCount3], [Signed1], [Signed2], [Signed3], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
    SELECT [EventId], [SessionId], [MediationHeld], [ReasonNotHeld], [EventDate], [MediatorId], [MediatorType], [Comments], [AgreementType], [AgreementSubmittedParties], [AgreementPreparedAttorney], [AgreementSigned], [AdjournedTimeRemaining], [TimeRemaining], [SignedCount1], [SignedCount2], [SignedCount3], [Signed1], [Signed2], [Signed3], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_events;
    PRINT 'tjc_modules_med_events: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
    SET IDENTITY_INSERT dbo.tjc_modules_med_events OFF;
  END
  ELSE PRINT 'tjc_modules_med_events: skipped (already has data)';

  IF NOT EXISTS (SELECT 1 FROM dbo.tjc_modules_med_issue_by_session)
  BEGIN
    INSERT INTO dbo.tjc_modules_med_issue_by_session ([SessionId], [IssueId], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById])
    SELECT [SessionId], [IssueId], [CreatedDate], [CreatedById], [LastModifiedDate], [LastModifiedById] FROM dbo.tjc_med_issue_by_session;
    PRINT 'tjc_modules_med_issue_by_session: ' + CAST(@@ROWCOUNT AS varchar(20)) + ' row(s) copied';
  END
  ELSE PRINT 'tjc_modules_med_issue_by_session: skipped (already has data)';

END
ELSE PRINT 'Historical cases, sessions and events not copied (@IncludeHistory = 0).';

COMMIT TRANSACTION;
