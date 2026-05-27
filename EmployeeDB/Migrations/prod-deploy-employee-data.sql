/*=============================================================================
  PROD DEPLOYMENT: Migrate employee data from legacy intranet -> DAL2 schema.

  Source server : 10.212.72.62
  Source DB     : intranet                (legacy Emp_* tables)
  Target DB     : intranet.jud12.local    (DAL2 tjc_* tables)
  Credentials   : SQL login intranet_web_user / intranet_web_user

  How this differs from migrate-employeedb.sql (the dev refresh script):
    The dev script PRESERVES lookup IDs from the old DB -- it INSERTs the
    counties / departments / classes / etc. into the new lookup tables
    using the same primary keys. That's appropriate when the new DB is
    empty and you want a 1-for-1 clone of the legacy data.

    This PROD script treats the new DB's lookup tables as the canonical
    source of truth. It does NOT touch them. Instead, every foreign key
    on the old employee data is TRANSLATED by matching the legacy
    description / name against the corresponding NEW lookup row, and the
    new ID is used. When a description has no match in the new DB the row
    is still migrated but the FK is set NULL and the unmatched value is
    captured in dbo.tjc_employee_migration_exception so an HR admin can
    follow up.

  Lookup translations (old -> new):
    Emp_Counties      .County              -> tjc_gl_counties              .CountyName
    Emp_Divisions     .DivisionUnitName    -> tjc_gl_group                 .GroupName
    Emp_JobCategories .JobCategory         -> tjc_employee_job_group       .Description
    Emp_Classes       .ClassName           -> tjc_employee_class           .ClassName
    Emp_Locations     .LocationName        -> tjc_employee_office_location .Description
    Emp_Groups        .GroupName           -> tjc_gl_group                 .GroupName

  Field renames inside Emp_Employees -> tjc_employee:
    DivisionUnitId    -> DepartmentId        (FK translated via Emp_Divisions)
    JobCategoryId     -> JobGroupId          (FK translated via Emp_JobCategories)
    EmailWork         -> Email
    EmailHome         -> PersonalEmail
    Address1+Address2 -> Address             (newline-joined)
    Location          -> OfficeLocationId    (FK translated by name)
    County            -> CountyId            (FK translated by name)
    FileID            -> FileId              (case-only)
    TerminatedDate    -> TerminationDate
    DateOfBirth       -> BirthDate
    Title             -> JobTitle
    StateCounty       -> AgencyOfEmployment
    Active            -> IsActive
  Dropped: Pager / BBPin / BBPinLabel / PhotoUrl (no DAL2 column).
  Phone columns on Emp_Employees (PhoneHome / PhoneCell / Phone / Pager)
  are migrated as separate tjc_employee_phone rows.

  How to run:
    sqlcmd -S 10.212.72.62 -U intranet_web_user -P intranet_web_user \
           -i prod-deploy-employee-data.sql
    -- or paste into SSMS connected to the same server.

  Re-runnability:
    The script uses INSERT ... WHERE NOT EXISTS everywhere, so running it
    a second time is a no-op for rows that already migrated. The exception
    table is DROPped and recreated each run.

  Transactional safety:
    Everything runs inside one BEGIN TRAN / COMMIT. SET XACT_ABORT ON
    auto-rolls-back on any error.
=============================================================================*/

USE [intranet.jud12.local];
GO

SET XACT_ABORT ON;
SET NOCOUNT ON;

-- All migrated rows carry CreatedById / LastModifiedById = -1 (sentinel for
-- "automated migration" -- matches the convention used by the dev script and
-- the controller layer's default userId fallback).
DECLARE @SystemUser INT = -1;

BEGIN TRAN;

/*=============================================================================
  0. Exception table
  ----------------------------------------------------------------------------
  DROPped and recreated on every run so the report reflects the current
  state, not history. Each row identifies one source-record + one issue:
    SourceTable  : 'Emp_Employees', 'Emp_Phones', 'Emp_GroupMemberships', ...
    SourceKey    : e.g. 'EmployeeId=993'  or  'EmployeeId=993, PhoneType=Mobile'
    Issue        : human-readable, e.g. 'Unmatched County: "Foo"'
=============================================================================*/
IF OBJECT_ID('dbo.tjc_employee_migration_exception', 'U') IS NOT NULL
    DROP TABLE dbo.tjc_employee_migration_exception;

CREATE TABLE dbo.tjc_employee_migration_exception (
    ExceptionId INT IDENTITY(1,1) PRIMARY KEY,
    SourceTable VARCHAR(64)    NOT NULL,
    SourceKey   NVARCHAR(200)  NULL,
    Issue       NVARCHAR(1000) NOT NULL,
    LogDate     DATETIME       NOT NULL DEFAULT GETDATE()
);
PRINT 'Created/reset dbo.tjc_employee_migration_exception';

/*=============================================================================
  1. Pre-flight: log every legacy employee whose lookup descriptions don't
     match anything in the target lookups. The migration in step 2 will still
     run for these employees -- their FK columns just end up NULL -- but this
     pre-flight surfaces the data-cleanup work before the migration overwrites
     anything.

     All description comparisons trim whitespace; collation is already
     case-insensitive on this server.
=============================================================================*/

-- 1a. County (Emp_Employees.County is a free-text varchar in the legacy DB)
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_Employees',
       'EmployeeId=' + CAST(e.EmployeeId AS VARCHAR(10)),
       'Unmatched County: "' + LTRIM(RTRIM(e.County)) + '"'
FROM intranet.dbo.Emp_Employees e
WHERE e.County IS NOT NULL AND LTRIM(RTRIM(e.County)) <> ''
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_gl_counties n
      WHERE LTRIM(RTRIM(n.CountyName)) = LTRIM(RTRIM(e.County))
  );

-- 1b. Department (employee.DivisionUnitId -> Emp_Divisions.DivisionUnitName -> tjc_gl_group.GroupName)
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_Employees',
       'EmployeeId=' + CAST(e.EmployeeId AS VARCHAR(10)),
       'Unmatched Department: "' + LTRIM(RTRIM(d.DivisionUnitName)) + '"'
FROM intranet.dbo.Emp_Employees e
INNER JOIN intranet.dbo.Emp_Divisions d ON d.DivisionUnitId = e.DivisionUnitId
WHERE e.DivisionUnitId IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_gl_group n
      WHERE LTRIM(RTRIM(n.GroupName)) = LTRIM(RTRIM(d.DivisionUnitName))
  );

-- 1c. Job Group (employee.JobCategoryId -> Emp_JobCategories.JobCategory -> tjc_employee_job_group.Description)
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_Employees',
       'EmployeeId=' + CAST(e.EmployeeId AS VARCHAR(10)),
       'Unmatched JobGroup: "' + LTRIM(RTRIM(j.JobCategory)) + '"'
FROM intranet.dbo.Emp_Employees e
INNER JOIN intranet.dbo.Emp_JobCategories j ON j.JobCategoryId = e.JobCategoryId
WHERE e.JobCategoryId IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_job_group n
      WHERE LTRIM(RTRIM(n.Description)) = LTRIM(RTRIM(j.JobCategory))
  );

-- 1d. Class (employee.ClassId -> Emp_Classes.ClassName -> tjc_employee_class.ClassName).
--     We match on ClassName, not ClassCode, because the code type changed
--     (varchar(5) -> int) and not every legacy code is numeric.
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_Employees',
       'EmployeeId=' + CAST(e.EmployeeId AS VARCHAR(10)),
       'Unmatched Class: "' + LTRIM(RTRIM(c.ClassName)) + '"'
FROM intranet.dbo.Emp_Employees e
INNER JOIN intranet.dbo.Emp_Classes c ON c.ClassId = e.ClassId
WHERE e.ClassId IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_class n
      WHERE LTRIM(RTRIM(n.ClassName)) = LTRIM(RTRIM(c.ClassName))
  );

-- 1e. Office location (employee.Location is a free-text varchar in legacy DB)
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_Employees',
       'EmployeeId=' + CAST(e.EmployeeId AS VARCHAR(10)),
       'Unmatched OfficeLocation: "' + LTRIM(RTRIM(e.Location)) + '"'
FROM intranet.dbo.Emp_Employees e
WHERE e.Location IS NOT NULL AND LTRIM(RTRIM(e.Location)) <> ''
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_office_location n
      WHERE LTRIM(RTRIM(n.Description)) = LTRIM(RTRIM(e.Location))
  );

-- 1f. Race (employee.Race is the RaceCode itself -- match against tjc_employee_race.RaceCode)
--      Skip legacy values that aren't real race codes: NUL byte (ASCII 0),
--      single regular space, tab, or any other control char. The legacy DB
--      has ~70 employees with Race = CHAR(0) which is a "no race recorded"
--      sentinel; logging those as exceptions is just noise.
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_Employees',
       'EmployeeId=' + CAST(e.EmployeeId AS VARCHAR(10)),
       'Unmatched Race code: "' + LTRIM(RTRIM(e.Race)) + '"'
FROM intranet.dbo.Emp_Employees e
WHERE e.Race IS NOT NULL
  AND LTRIM(RTRIM(e.Race)) <> ''
  AND ASCII(LEFT(e.Race, 1)) > 32                     -- ignore NUL / control / space
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_race n
      WHERE LTRIM(RTRIM(n.RaceCode)) = LTRIM(RTRIM(e.Race))
  );

-- PRINT can't take a subquery directly; capture the count into a variable first.
DECLARE @PreflightExceptionCount INT;
SELECT @PreflightExceptionCount = COUNT(*) FROM dbo.tjc_employee_migration_exception;
PRINT CONCAT('Pre-flight exceptions logged: ', @PreflightExceptionCount);

/*=============================================================================
  2. tjc_employee
  ----------------------------------------------------------------------------
  Preserves EmployeeId (IDENTITY_INSERT). The PK propagates to phones,
  emergency contacts, and group memberships so we need it stable.

  Every FK column uses a correlated subquery to translate the legacy ID
  into the matching new ID (or NULL if no match).
=============================================================================*/
SET IDENTITY_INSERT dbo.tjc_employee ON;

INSERT INTO dbo.tjc_employee (
    EmployeeId, UserId, SupervisorId, DepartmentId, JobGroupId, ClassId, BadgeNumber,
    Position, EmploymentType, FirstName, LastName, MiddleInitial, Email, PersonalEmail,
    Address, City, State, Zip, OfficeLocationId, CountyId, FileId,
    HireDate, TerminationDate, ServiceDate, BirthDate, Race, Gender, JobTitle, Salary,
    AnnualLeaveBalance, SickLeaveBalance, SocialSecurityNumber, AgencyOfEmployment,
    IsActive, IsEmployee, ManateeAccess, SarasotaAccess, DesotoAccess,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.EmployeeId,
    o.UserId,
    o.SupervisorId,
    /* DepartmentId  <- old DivisionUnitId, translated by division name */
    (SELECT TOP 1 g.GroupID
       FROM dbo.tjc_gl_group g
       INNER JOIN intranet.dbo.Emp_Divisions d
         ON LTRIM(RTRIM(g.GroupName)) = LTRIM(RTRIM(d.DivisionUnitName))
       WHERE d.DivisionUnitId = o.DivisionUnitId),
    /* JobGroupId    <- old JobCategoryId, translated by category name */
    (SELECT TOP 1 j.JobGroupId
       FROM dbo.tjc_employee_job_group j
       INNER JOIN intranet.dbo.Emp_JobCategories jc
         ON LTRIM(RTRIM(j.Description)) = LTRIM(RTRIM(jc.JobCategory))
       WHERE jc.JobCategoryId = o.JobCategoryId),
    /* ClassId       <- old ClassId, translated by ClassName */
    (SELECT TOP 1 c.ClassId
       FROM dbo.tjc_employee_class c
       INNER JOIN intranet.dbo.Emp_Classes oc
         ON LTRIM(RTRIM(c.ClassName)) = LTRIM(RTRIM(oc.ClassName))
       WHERE oc.ClassId = o.ClassId),
    NULL,                                                   -- BadgeNumber: new column, no legacy source
    o.Position,
    o.EmploymentType,
    o.FirstName, o.LastName, o.MiddleInitial,
    o.EmailWork,                                            -- -> Email
    o.EmailHome,                                            -- -> PersonalEmail
    /* Address = Address1 + (newline + Address2 if present). The Edit form's
       SplitAddressLines helper unpacks this back into two text boxes. */
    NULLIF(LTRIM(RTRIM(
        COALESCE(NULLIF(LTRIM(RTRIM(o.Address1)), ''), '') +
        CASE WHEN o.Address2 IS NULL OR LTRIM(RTRIM(o.Address2)) = ''
             THEN ''
             ELSE CHAR(10) + LTRIM(RTRIM(o.Address2)) END
    )), ''),
    o.City, o.State, o.Zip,
    /* OfficeLocationId <- old Location varchar, matched against Description */
    (SELECT TOP 1 l.OfficeLocationId
       FROM dbo.tjc_employee_office_location l
       WHERE LTRIM(RTRIM(l.Description)) = LTRIM(RTRIM(o.Location))),
    /* CountyId         <- old County varchar, matched against CountyName */
    (SELECT TOP 1 cc.CountyId
       FROM dbo.tjc_gl_counties cc
       WHERE LTRIM(RTRIM(cc.CountyName)) = LTRIM(RTRIM(o.County))),
    o.FileID,                                               -- -> FileId (DNN files FK; preserved as-is)
    o.HireDate,
    o.TerminatedDate,                                       -- -> TerminationDate
    o.ServiceDate,
    o.DateOfBirth,                                          -- -> BirthDate
    o.Race, o.Gender,
    o.Title,                                                -- -> JobTitle
    o.Salary,
    o.AnnualLeaveBalance, o.SickLeaveBalance,
    o.SocialSecurityNumber,
    o.StateCounty,                                          -- -> AgencyOfEmployment
    o.Active,                                               -- -> IsActive
    o.IsEmployee,
    o.ManateeAccess, o.SarasotaAccess, o.DesotoAccess,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_Employees o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee n WHERE n.EmployeeId = o.EmployeeId
);

SET IDENTITY_INSERT dbo.tjc_employee OFF;
PRINT CONCAT('tjc_employee: inserted ', @@ROWCOUNT, ' rows');

/*=============================================================================
  3. tjc_employee_phone
  ----------------------------------------------------------------------------
  3a. The dedicated Emp_Phones rows (the only ones the new app reads).
  3b. The denormalized Phone* columns on Emp_Employees (PhoneHome,
      PhoneCell, Phone+Extension, Pager) -- each becomes its own row so
      the legacy data isn't lost.

  All phone inserts dedupe on (EmployeeId, PhoneType, PhoneNumber) so a
  re-run is safe AND we don't double-create phones that already exist
  via the dedicated table.
=============================================================================*/

-- 3a. From Emp_Phones (preserves SWN flags, cascade order, location).
INSERT INTO dbo.tjc_employee_phone (
    EmployeeId, OfficeLocationId, PhoneType, PhoneNumber, Extension, IsMain,
    PhoneCascade, SwnText, SwnCall, SwnExcludeExtension,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.EmployeeId,
    (SELECT TOP 1 l.OfficeLocationId
       FROM dbo.tjc_employee_office_location l
       WHERE LTRIM(RTRIM(l.Description)) = LTRIM(RTRIM(o.Location))),
    o.PhoneType,
    o.PhoneNumber,
    o.Extension,
    0,                                                      -- IsMain (legacy had no "main" flag)
    TRY_CAST(o.PhoneCascade AS INT),                        -- nvarchar(1) -> int
    ISNULL(o.SWNText, 0),
    ISNULL(o.SWNCall, 0),
    ISNULL(o.SWNExcludeExtension, 0),
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_Phones o
WHERE EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_phone n
      WHERE n.EmployeeId = o.EmployeeId
        AND ISNULL(n.PhoneType, '')   = ISNULL(o.PhoneType, '')
        AND ISNULL(n.PhoneNumber, '') = ISNULL(o.PhoneNumber, '')
  );
PRINT CONCAT('tjc_employee_phone (from Emp_Phones): inserted ', @@ROWCOUNT, ' rows');

-- Log Emp_Phones rows whose Location string didn't match any office location.
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_Phones',
       'PhoneId=' + CAST(o.PhoneId AS VARCHAR(10)) + ', EmployeeId=' + CAST(o.EmployeeId AS VARCHAR(10)),
       'Unmatched Phone Location: "' + LTRIM(RTRIM(o.Location)) + '"'
FROM intranet.dbo.Emp_Phones o
WHERE o.Location IS NOT NULL AND LTRIM(RTRIM(o.Location)) <> ''
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_office_location n
      WHERE LTRIM(RTRIM(n.Description)) = LTRIM(RTRIM(o.Location))
  );

-- 3b. From the denormalized columns on Emp_Employees.
INSERT INTO dbo.tjc_employee_phone (
    EmployeeId, PhoneType, PhoneNumber, IsMain, SwnText, SwnCall, SwnExcludeExtension,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, 'Home', o.PhoneHome, 0, 0, 0, 0,
       GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_Employees o
WHERE o.PhoneHome IS NOT NULL AND LTRIM(RTRIM(o.PhoneHome)) <> ''
  AND EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_phone n
      WHERE n.EmployeeId = o.EmployeeId AND n.PhoneType = 'Home' AND n.PhoneNumber = o.PhoneHome
  );
PRINT CONCAT('tjc_employee_phone (Home from Employees): inserted ', @@ROWCOUNT, ' rows');

INSERT INTO dbo.tjc_employee_phone (
    EmployeeId, PhoneType, PhoneNumber, IsMain, SwnText, SwnCall, SwnExcludeExtension,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, 'Mobile', o.PhoneCell, 0, 0, 0, 0,
       GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_Employees o
WHERE o.PhoneCell IS NOT NULL AND LTRIM(RTRIM(o.PhoneCell)) <> ''
  AND EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_phone n
      WHERE n.EmployeeId = o.EmployeeId AND n.PhoneType = 'Mobile' AND n.PhoneNumber = o.PhoneCell
  );
PRINT CONCAT('tjc_employee_phone (Mobile from Employees): inserted ', @@ROWCOUNT, ' rows');

INSERT INTO dbo.tjc_employee_phone (
    EmployeeId, PhoneType, PhoneNumber, Extension, IsMain, SwnText, SwnCall, SwnExcludeExtension,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, 'Work', o.Phone, o.Extension, 1, 0, 0, 0,
       GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_Employees o
WHERE o.Phone IS NOT NULL AND LTRIM(RTRIM(o.Phone)) <> ''
  AND EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_phone n
      WHERE n.EmployeeId = o.EmployeeId AND n.PhoneType = 'Work' AND n.PhoneNumber = o.Phone
  );
PRINT CONCAT('tjc_employee_phone (Work from Employees): inserted ', @@ROWCOUNT, ' rows');

INSERT INTO dbo.tjc_employee_phone (
    EmployeeId, PhoneType, PhoneNumber, IsMain, SwnText, SwnCall, SwnExcludeExtension,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, 'Pager', o.Pager, 0, 0, 0, 0,
       GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_Employees o
WHERE o.Pager IS NOT NULL AND LTRIM(RTRIM(o.Pager)) <> ''
  AND EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_phone n
      WHERE n.EmployeeId = o.EmployeeId AND n.PhoneType = 'Pager' AND n.PhoneNumber = o.Pager
  );
PRINT CONCAT('tjc_employee_phone (Pager from Employees): inserted ', @@ROWCOUNT, ' rows');

/*=============================================================================
  4. tjc_employee_group_membership
  ----------------------------------------------------------------------------
  Translate old GroupId -> new GroupID by joining old Emp_Groups to the new
  tjc_gl_group on GroupName. Unmatched legacy GroupIds get logged AND
  SKIPPED (GroupId is part of the composite PK so we can't store NULL).
=============================================================================*/

-- Log unmatched group names.
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_GroupMemberships',
       'EmployeeId=' + CAST(gm.EmployeeId AS VARCHAR(10)) + ', GroupId=' + CAST(gm.GroupId AS VARCHAR(10)),
       'Unmatched Group: "' + LTRIM(RTRIM(g.GroupName)) + '" (membership skipped)'
FROM intranet.dbo.Emp_GroupMemberships gm
INNER JOIN intranet.dbo.Emp_Groups g ON g.GroupId = gm.GroupId
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_gl_group n
    WHERE LTRIM(RTRIM(n.GroupName)) = LTRIM(RTRIM(g.GroupName))
);

INSERT INTO dbo.tjc_employee_group_membership (
    GroupId, EmployeeId, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    (SELECT TOP 1 n.GroupID FROM dbo.tjc_gl_group n
       WHERE LTRIM(RTRIM(n.GroupName)) = LTRIM(RTRIM(g.GroupName))),
    gm.EmployeeId,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_GroupMemberships gm
INNER JOIN intranet.dbo.Emp_Groups g ON g.GroupId = gm.GroupId
WHERE EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = gm.EmployeeId)
  AND EXISTS (SELECT 1 FROM dbo.tjc_gl_group n
              WHERE LTRIM(RTRIM(n.GroupName)) = LTRIM(RTRIM(g.GroupName)))
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_group_membership ngm
      INNER JOIN dbo.tjc_gl_group n ON n.GroupID = ngm.GroupId
      WHERE ngm.EmployeeId = gm.EmployeeId
        AND LTRIM(RTRIM(n.GroupName)) = LTRIM(RTRIM(g.GroupName))
  );
PRINT CONCAT('tjc_employee_group_membership: inserted ', @@ROWCOUNT, ' rows');

/*=============================================================================
  5. tjc_employee_emergency_contact   (no lookups; direct copy)
=============================================================================*/
INSERT INTO dbo.tjc_employee_emergency_contact (
    EmployeeId, FirstName, LastName, Relationship,
    PhoneHome, PhoneWork, PhoneMobile, CallOrder,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.EmployeeId, o.FirstName, o.LastName, o.Relationship,
    o.PhoneHome, o.PhoneWork, o.PhoneMobile, o.CallOrder,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_EmergencyContact o
WHERE EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tjc_employee_emergency_contact n
      WHERE n.EmployeeId = o.EmployeeId
        AND ISNULL(n.FirstName, '') = ISNULL(o.FirstName, '')
        AND ISNULL(n.LastName,  '') = ISNULL(o.LastName,  '')
  );
PRINT CONCAT('tjc_employee_emergency_contact: inserted ', @@ROWCOUNT, ' rows');

/*=============================================================================
  6. tjc_employee_position_history   (keyed by SSN, not EmployeeId)
  ----------------------------------------------------------------------------
  Field renames: ItemId -> PositionId (auto-generated, not preserved),
                 Position -> Description, InternalExternal -> IsInternal.
  Rating column is dropped (no DAL2 equivalent).
=============================================================================*/
INSERT INTO dbo.tjc_employee_position_history (
    SocialSecurityNumber, StartDate, EndDate, Description, IsInternal, EntryType,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.SocialSecurityNumber, o.StartDate, o.EndDate, o.Position,
    CASE WHEN UPPER(ISNULL(o.InternalExternal, '')) = 'INTERNAL' THEN 1 ELSE 0 END,
    o.EntryType,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_PositionHistorys o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_position_history n
    WHERE ISNULL(n.SocialSecurityNumber, '') = ISNULL(o.SocialSecurityNumber, '')
      AND ISNULL(n.Description, '')          = ISNULL(o.Position, '')
      AND ISNULL(n.StartDate, '1900-01-01')  = ISNULL(o.StartDate, '1900-01-01')
);
PRINT CONCAT('tjc_employee_position_history: inserted ', @@ROWCOUNT, ' rows');

/*=============================================================================
  7. tjc_employee_service_history   (keyed by SSN; Company -> CompanyName)
=============================================================================*/
INSERT INTO dbo.tjc_employee_service_history (
    SocialSecurityNumber, HireDate, TerminationDate, LastPayRate, CompanyName,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.SocialSecurityNumber, o.HireDate, o.TerminationDate, o.LastPayRate, o.Company,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_ServiceHistorys o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_service_history n
    WHERE n.SocialSecurityNumber = o.SocialSecurityNumber
      AND ISNULL(n.CompanyName, '') = ISNULL(o.Company, '')
      AND ISNULL(n.HireDate, '1900-01-01') = ISNULL(o.HireDate, '1900-01-01')
);
PRINT CONCAT('tjc_employee_service_history: inserted ', @@ROWCOUNT, ' rows');

/*=============================================================================
  8. tjc_employee_eeo
  ----------------------------------------------------------------------------
  Old Emp_EEO.JobGroup is actually a JobCategoryId in the legacy DB. We
  translate it the same way the employee table does -- via the category
  description. Unmatched JobGroup values are logged AND the EEO row is
  skipped (JobGroupId is required).
=============================================================================*/
INSERT INTO dbo.tjc_employee_migration_exception (SourceTable, SourceKey, Issue)
SELECT 'Emp_EEO',
       'Id=' + CAST(o.Id AS VARCHAR(10)) + ', Year=' + CAST(o.Year AS VARCHAR(10)),
       'Unmatched JobGroup id ' + CAST(o.JobGroup AS VARCHAR(10)) + ' (row skipped)'
FROM intranet.dbo.Emp_EEO o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_job_group j
    INNER JOIN intranet.dbo.Emp_JobCategories jc
      ON LTRIM(RTRIM(j.Description)) = LTRIM(RTRIM(jc.JobCategory))
    WHERE jc.JobCategoryId = o.JobGroup
);

INSERT INTO dbo.tjc_employee_eeo (
    JobGroupId,
    PopulationMale, PopulationFemale, PopulationWhite, PopulationIndian,
    PopulationBlack, PopulationAsian, PopulationHispanic, PopulationOther,
    HireMale, HireFemale, HireWhite, HireBlack, HireAsian, HireIndian, HireHispanic, HireOther,
    PromoMale, PromoFemale, PromoWhite, PromoBlack, PromoAsian, PromoIndian, PromoHispanic, PromoOther,
    TransferMale, TransferFemale, TransferWhite, TransferBlack,
    TransferAsian, TransferIndian, TransferHispanic, TransferOther,
    TermMale, TermFemale, TermWhite, TermBlack, TermIndian, TermAsian, TermHispanic, TermOther,
    Year,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    (SELECT TOP 1 j.JobGroupId
       FROM dbo.tjc_employee_job_group j
       INNER JOIN intranet.dbo.Emp_JobCategories jc
         ON LTRIM(RTRIM(j.Description)) = LTRIM(RTRIM(jc.JobCategory))
       WHERE jc.JobCategoryId = o.JobGroup),
    o.PopulationMale, o.PopulationFemale, o.PopulationWhite, o.PopulationIndian,
    o.PopulationBlack, o.PopulationAsian, o.PopulationHispanic, o.PopulationOther,
    o.HireMale, o.HireFemale, o.HireWhite, o.HireBlack, o.HireAsian, o.HireIndian, o.HireHispanic, o.HireOther,
    o.PromoMale, o.PromoFemale, o.PromoWhite, o.PromoBlack, o.PromoAsian, o.PromoIndian, o.PromoHispanic, o.PromoOther,
    o.TransferMale, o.TransferFemale, o.TransferWhite, o.TransferBlack,
    o.TransferAsian, o.TransferIndian, o.TransferHispanic, o.TransferOther,
    o.TermMale, o.TermFemale, o.TermWhite, o.TermBlack, o.TermIndian, o.TermAsian, o.TermHispanic, o.TermOther,
    o.Year,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet.dbo.Emp_EEO o
WHERE EXISTS (
    SELECT 1 FROM dbo.tjc_employee_job_group j
    INNER JOIN intranet.dbo.Emp_JobCategories jc
      ON LTRIM(RTRIM(j.Description)) = LTRIM(RTRIM(jc.JobCategory))
    WHERE jc.JobCategoryId = o.JobGroup
)
AND NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_eeo n
    INNER JOIN dbo.tjc_employee_job_group j ON j.JobGroupId = n.JobGroupId
    INNER JOIN intranet.dbo.Emp_JobCategories jc
      ON LTRIM(RTRIM(j.Description)) = LTRIM(RTRIM(jc.JobCategory))
    WHERE jc.JobCategoryId = o.JobGroup AND n.Year = o.Year
);
PRINT CONCAT('tjc_employee_eeo: inserted ', @@ROWCOUNT, ' rows');

/*=============================================================================
  9. Commit and report
=============================================================================*/
COMMIT TRAN;

PRINT '';
PRINT '==================== Post-migration row counts ====================';
SELECT 'tjc_employee'                   AS TableName, COUNT(*) AS [Count] FROM dbo.tjc_employee
UNION ALL SELECT 'tjc_employee_phone',                COUNT(*) FROM dbo.tjc_employee_phone
UNION ALL SELECT 'tjc_employee_emergency_contact',    COUNT(*) FROM dbo.tjc_employee_emergency_contact
UNION ALL SELECT 'tjc_employee_group_membership',     COUNT(*) FROM dbo.tjc_employee_group_membership
UNION ALL SELECT 'tjc_employee_position_history',     COUNT(*) FROM dbo.tjc_employee_position_history
UNION ALL SELECT 'tjc_employee_service_history',      COUNT(*) FROM dbo.tjc_employee_service_history
UNION ALL SELECT 'tjc_employee_eeo',                  COUNT(*) FROM dbo.tjc_employee_eeo;

PRINT '';
PRINT '==================== Exception summary ====================';
SELECT SourceTable, COUNT(*) AS ExceptionCount
FROM dbo.tjc_employee_migration_exception
GROUP BY SourceTable
ORDER BY SourceTable;

PRINT '';
PRINT '==================== Top 50 exception rows ====================';
SELECT TOP 50 ExceptionId, SourceTable, SourceKey, Issue
FROM dbo.tjc_employee_migration_exception
ORDER BY SourceTable, ExceptionId;

PRINT 'Migration complete. Inspect dbo.tjc_employee_migration_exception for the full list.';
GO
