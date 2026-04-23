/*============================================================================
  EmployeeDB Data Migration
  Source:  intranet_613.dbo.Emp_*
  Target:  intranet.jud12.local.dbo.tjc_employee_* / tjc_gl_*

  Purpose: Fill gaps in the already-partially-migrated target DB. Uses
  INSERT ... SELECT WHERE NOT EXISTS patterns so re-running is safe.

  Run this against the intranet.jud12.local database.
============================================================================*/

USE [intranet.jud12.local];
GO

SET XACT_ABORT ON;
SET NOCOUNT ON;

DECLARE @SystemUser INT = -1;      -- CreatedById/LastModifiedById for migrated rows

BEGIN TRAN;

-----------------------------------------------------------------------------
-- 0. Create tjc_employee_assigned_item if it doesn't exist, then load data
-----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'tjc_employee_assigned_item')
BEGIN
    CREATE TABLE dbo.tjc_employee_assigned_item (
        ItemId            INT IDENTITY(1,1) NOT NULL,
        EmployeeId        INT NOT NULL,
        ItemType          NVARCHAR(25) NULL,
        ItemName          NVARCHAR(50) NULL,
        CreatedDate       DATETIME NOT NULL CONSTRAINT DF_tjc_employee_assigned_item_CreatedDate DEFAULT GETDATE(),
        CreatedById       INT NOT NULL,
        LastModifiedDate  DATETIME NOT NULL CONSTRAINT DF_tjc_employee_assigned_item_LastModifiedDate DEFAULT GETDATE(),
        LastModifiedById  INT NOT NULL,
        CONSTRAINT PK_tjc_employee_assigned_item PRIMARY KEY CLUSTERED (ItemId)
    );
END

-- Insert any rows from old Emp_AssignedItems that aren't in the new table.
-- Match by EmployeeId + ItemName since the new table has its own identity.
INSERT INTO dbo.tjc_employee_assigned_item (EmployeeId, ItemType, ItemName, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, o.ItemType, o.ItemName, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_AssignedItems o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_assigned_item n
    WHERE n.EmployeeId = o.EmployeeId AND ISNULL(n.ItemName,'') = ISNULL(o.ItemName,'')
);
PRINT CONCAT('tjc_employee_assigned_item: inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- 1. Lookup tables: tjc_gl_counties, tjc_gl_group, tjc_employee_race,
--    tjc_employee_job_group, tjc_employee_office_location, tjc_employee_class
--    Insert any rows from old DB that don't already exist (match on name).
-----------------------------------------------------------------------------

-- tjc_gl_counties (from Emp_Counties). 4 rows already exist; only add missing.
SET IDENTITY_INSERT dbo.tjc_gl_counties ON;
INSERT INTO dbo.tjc_gl_counties (CountyId, CountyName, CreatedById, LastModifiedById, CreatedDate, LastModifiedDate)
SELECT o.CountyId, o.County, CONVERT(NVARCHAR(50), @SystemUser), CONVERT(NVARCHAR(50), @SystemUser), GETDATE(), GETDATE()
FROM intranet_613.dbo.Emp_Counties o
WHERE NOT EXISTS (SELECT 1 FROM dbo.tjc_gl_counties n WHERE n.CountyName = o.County)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_gl_counties n WHERE n.CountyId = o.CountyId);
SET IDENTITY_INSERT dbo.tjc_gl_counties OFF;
PRINT CONCAT('tjc_gl_counties: inserted ', @@ROWCOUNT, ' rows');

-- tjc_gl_group (from Emp_Groups). Match on name so we don't duplicate.
SET IDENTITY_INSERT dbo.tjc_gl_group ON;
INSERT INTO dbo.tjc_gl_group (GroupID, GroupName, GroupType, IsSwnGroup, CreatedDate, CreatedByID, LastModifiedDate, LastModifiedByID)
SELECT o.GroupId, o.GroupName, o.GroupType, ISNULL(o.IsSWNGroup, 0), GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Groups o
WHERE NOT EXISTS (SELECT 1 FROM dbo.tjc_gl_group n WHERE n.GroupName = o.GroupName)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_gl_group n WHERE n.GroupID = o.GroupId);
SET IDENTITY_INSERT dbo.tjc_gl_group OFF;
PRINT CONCAT('tjc_gl_group: inserted ', @@ROWCOUNT, ' rows');

-- tjc_employee_race (from Emp_Race). Match on RaceCode.
SET IDENTITY_INSERT dbo.tjc_employee_race ON;
INSERT INTO dbo.tjc_employee_race (RaceId, RaceCode, Description, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.RaceId, o.RaceCode, o.Race, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Race o
WHERE NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_race n WHERE n.RaceCode = o.RaceCode)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_race n WHERE n.RaceId = o.RaceId);
SET IDENTITY_INSERT dbo.tjc_employee_race OFF;
PRINT CONCAT('tjc_employee_race: inserted ', @@ROWCOUNT, ' rows');

-- tjc_employee_job_group (from Emp_JobCategories). Match on name.
SET IDENTITY_INSERT dbo.tjc_employee_job_group ON;
INSERT INTO dbo.tjc_employee_job_group (JobGroupId, Description, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.JobCategoryId, o.JobCategory, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_JobCategories o
WHERE NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_job_group n WHERE n.Description = o.JobCategory)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_job_group n WHERE n.JobGroupId = o.JobCategoryId);
SET IDENTITY_INSERT dbo.tjc_employee_job_group OFF;
PRINT CONCAT('tjc_employee_job_group: inserted ', @@ROWCOUNT, ' rows');

-- tjc_employee_office_location (from Emp_Locations). Match on name.
SET IDENTITY_INSERT dbo.tjc_employee_office_location ON;
INSERT INTO dbo.tjc_employee_office_location (OfficeLocationId, Description, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.LocationID, o.LocationName, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Locations o
WHERE NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_office_location n WHERE n.Description = o.LocationName)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_office_location n WHERE n.OfficeLocationId = o.LocationID);
SET IDENTITY_INSERT dbo.tjc_employee_office_location OFF;
PRINT CONCAT('tjc_employee_office_location: inserted ', @@ROWCOUNT, ' rows');

-- tjc_employee_class (from Emp_Classes). ClassCode varchar(5) -> int (TRY_CAST).
-- Match on ClassId; skip rows where ClassCode cannot be cast to int.
SET IDENTITY_INSERT dbo.tjc_employee_class ON;
INSERT INTO dbo.tjc_employee_class (ClassId, ClassName, ClassCode, PayGrade, FLSA, EEO, MMax, MMin, AMax, AMin,
                                     CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.ClassId, o.ClassName,
       ISNULL(TRY_CAST(o.ClassCode AS INT), 0),
       TRY_CAST(o.PayGrade AS INT),
       o.FLSA,
       TRY_CAST(o.EEO AS INT),
       o.MMax, o.MMin, o.AMax, o.AMin,
       GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Classes o
WHERE NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_class n WHERE n.ClassId = o.ClassId);
SET IDENTITY_INSERT dbo.tjc_employee_class OFF;
PRINT CONCAT('tjc_employee_class: inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- 2. tjc_employee (the 82 missing employees from Emp_Employees)
--    Preserve EmployeeId so that FK references in other tables line up.
-----------------------------------------------------------------------------
SET IDENTITY_INSERT dbo.tjc_employee ON;
INSERT INTO dbo.tjc_employee (
    EmployeeId, UserId, SupervisorId, DepartmentId, JobGroupId, ClassId, BadgeNumber,
    Position, EmploymentType, FirstName, LastName, MiddleInitial, Email, PersonalEmail,
    Address, City, State, Zip, OfficeLocationId, LocationName, CountyId, PhotoFileId,
    HireDate, TerminationDate, ServiceDate, BirthDate, Race, Gender, JobTitle, Salary,
    AnnualLeaveBalance, SickLeaveBalance, SocialSecurityNumber, AgencyOfEmployment,
    IsActive, IsEmployee, ManateeAccess, SarasotaAccess, DesotoAccess, SwnGroupId,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById
)
SELECT
    o.EmployeeId,
    o.UserId,
    o.SupervisorId,
    o.DivisionUnitId,                                       -- -> DepartmentId
    o.JobCategoryId,                                        -- -> JobGroupId
    o.ClassId,
    NULL,                                                   -- BadgeNumber new
    o.Position,
    o.EmploymentType,
    o.FirstName,
    o.LastName,
    o.MiddleInitial,
    o.EmailWork,                                            -- -> Email
    o.EmailHome,                                            -- -> PersonalEmail
    LTRIM(RTRIM(COALESCE(o.Address1, '') +
                CASE WHEN o.Address2 IS NULL OR o.Address2 = '' THEN '' ELSE ', ' + o.Address2 END)),
    o.City, o.State, o.Zip,
    (SELECT TOP 1 l.OfficeLocationId FROM dbo.tjc_employee_office_location l WHERE l.Description = o.Location),
    o.Location,                                             -- keep denormalized copy
    (SELECT TOP 1 c.CountyId FROM dbo.tjc_gl_counties c WHERE c.CountyName = o.County),
    NULL,                                                   -- PhotoFileId (path doesn't map)
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
    o.ManateeAccess,
    o.SarasotaAccess, o.DesotoAccess,
    NULL,                                                   -- SwnGroupId new
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Employees o
WHERE NOT EXISTS (SELECT 1 FROM dbo.tjc_employee n WHERE n.EmployeeId = o.EmployeeId);
SET IDENTITY_INSERT dbo.tjc_employee OFF;
PRINT CONCAT('tjc_employee: inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- 3. tjc_employee_phone — normalize phone data from old Emp_Phones + the
--    denormalized columns on old Emp_Employees (PhoneHome, PhoneCell, Phone,
--    Pager). Only insert rows whose (EmployeeId, PhoneType, PhoneNumber)
--    combination doesn't already exist.
-----------------------------------------------------------------------------

-- 3a. From Emp_Phones (the separate phone table in old DB).
INSERT INTO dbo.tjc_employee_phone (
    EmployeeId, OfficeLocationId, PhoneType, PhoneNumber, Extension, IsMain,
    PhoneCascade, SwnText, SwnCall, SwnExcludeExtension,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.EmployeeId,
    (SELECT TOP 1 l.OfficeLocationId FROM dbo.tjc_employee_office_location l WHERE l.Description = o.Location),
    o.PhoneType,
    o.PhoneNumber,
    o.Extension,
    0,
    TRY_CAST(o.PhoneCascade AS INT),
    ISNULL(o.SWNText, 0),
    ISNULL(o.SWNCall, 0),
    ISNULL(o.SWNExcludeExtension, 0),
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Phones o
WHERE EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_phone n
    WHERE n.EmployeeId = o.EmployeeId
      AND ISNULL(n.PhoneType,'') = ISNULL(o.PhoneType,'')
      AND ISNULL(n.PhoneNumber,'') = ISNULL(o.PhoneNumber,'')
  );
PRINT CONCAT('tjc_employee_phone (from Emp_Phones): inserted ', @@ROWCOUNT, ' rows');

-- 3b. From denormalized Emp_Employees phone columns (PhoneHome, PhoneCell, Phone, Pager).
-- Each becomes its own row. Only for employees that are in both DBs.
INSERT INTO dbo.tjc_employee_phone (EmployeeId, PhoneType, PhoneNumber, IsMain, SwnText, SwnCall, SwnExcludeExtension,
                                     CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, 'Home', o.PhoneHome, 0, 0, 0, 0, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Employees o
WHERE o.PhoneHome IS NOT NULL AND LTRIM(RTRIM(o.PhoneHome)) <> ''
  AND EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_phone n WHERE n.EmployeeId = o.EmployeeId AND n.PhoneType = 'Home' AND n.PhoneNumber = o.PhoneHome);
PRINT CONCAT('tjc_employee_phone (Home from Employees): inserted ', @@ROWCOUNT, ' rows');

INSERT INTO dbo.tjc_employee_phone (EmployeeId, PhoneType, PhoneNumber, IsMain, SwnText, SwnCall, SwnExcludeExtension,
                                     CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, 'Mobile', o.PhoneCell, 0, 0, 0, 0, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Employees o
WHERE o.PhoneCell IS NOT NULL AND LTRIM(RTRIM(o.PhoneCell)) <> ''
  AND EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_phone n WHERE n.EmployeeId = o.EmployeeId AND n.PhoneType = 'Mobile' AND n.PhoneNumber = o.PhoneCell);
PRINT CONCAT('tjc_employee_phone (Mobile from Employees): inserted ', @@ROWCOUNT, ' rows');

INSERT INTO dbo.tjc_employee_phone (EmployeeId, PhoneType, PhoneNumber, Extension, IsMain, SwnText, SwnCall, SwnExcludeExtension,
                                     CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, 'Work', o.Phone, o.Extension, 1, 0, 0, 0, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Employees o
WHERE o.Phone IS NOT NULL AND LTRIM(RTRIM(o.Phone)) <> ''
  AND EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_phone n WHERE n.EmployeeId = o.EmployeeId AND n.PhoneType = 'Work' AND n.PhoneNumber = o.Phone);
PRINT CONCAT('tjc_employee_phone (Work from Employees): inserted ', @@ROWCOUNT, ' rows');

INSERT INTO dbo.tjc_employee_phone (EmployeeId, PhoneType, PhoneNumber, IsMain, SwnText, SwnCall, SwnExcludeExtension,
                                     CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.EmployeeId, 'Pager', o.Pager, 0, 0, 0, 0, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_Employees o
WHERE o.Pager IS NOT NULL AND LTRIM(RTRIM(o.Pager)) <> ''
  AND EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_phone n WHERE n.EmployeeId = o.EmployeeId AND n.PhoneType = 'Pager' AND n.PhoneNumber = o.Pager);
PRINT CONCAT('tjc_employee_phone (Pager from Employees): inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- 4. tjc_employee_group_membership (from Emp_GroupMemberships)
-----------------------------------------------------------------------------
INSERT INTO dbo.tjc_employee_group_membership (GroupId, EmployeeId, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT o.GroupId, o.EmployeeId, GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_GroupMemberships o
WHERE EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND EXISTS (SELECT 1 FROM dbo.tjc_gl_group g WHERE g.GroupID = o.GroupId)
  AND NOT EXISTS (SELECT 1 FROM dbo.tjc_employee_group_membership n WHERE n.GroupId = o.GroupId AND n.EmployeeId = o.EmployeeId);
PRINT CONCAT('tjc_employee_group_membership: inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- 5. tjc_employee_emergency_contact (from Emp_EmergencyContact)
-----------------------------------------------------------------------------
INSERT INTO dbo.tjc_employee_emergency_contact (
    EmployeeId, FirstName, LastName, Relationship, PhoneHome, PhoneWork, PhoneMobile, CallOrder,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.EmployeeId, o.FirstName, o.LastName, o.Relationship, o.PhoneHome, o.PhoneWork, o.PhoneMobile, o.CallOrder,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_EmergencyContact o
WHERE EXISTS (SELECT 1 FROM dbo.tjc_employee e WHERE e.EmployeeId = o.EmployeeId)
  AND NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_emergency_contact n
    WHERE n.EmployeeId = o.EmployeeId
      AND ISNULL(n.FirstName,'') = ISNULL(o.FirstName,'')
      AND ISNULL(n.LastName,'') = ISNULL(o.LastName,'')
  );
PRINT CONCAT('tjc_employee_emergency_contact: inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- 6. tjc_employee_position_history (from Emp_PositionHistorys)
-----------------------------------------------------------------------------
INSERT INTO dbo.tjc_employee_position_history (
    SocialSecurityNumber, StartDate, EndDate, Description, IsInternal, EntryType,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.SocialSecurityNumber, o.StartDate, o.EndDate, o.Position,
    CASE WHEN UPPER(ISNULL(o.InternalExternal,'')) = 'INTERNAL' THEN 1 ELSE 0 END,
    o.EntryType,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_PositionHistorys o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_position_history n
    WHERE ISNULL(n.SocialSecurityNumber,'') = ISNULL(o.SocialSecurityNumber,'')
      AND ISNULL(n.Description,'') = ISNULL(o.Position,'')
      AND ISNULL(n.StartDate, '1900-01-01') = ISNULL(o.StartDate, '1900-01-01')
  );
PRINT CONCAT('tjc_employee_position_history: inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- 7. tjc_employee_service_history (from Emp_ServiceHistorys)
-----------------------------------------------------------------------------
INSERT INTO dbo.tjc_employee_service_history (
    SocialSecurityNumber, HireDate, TerminationDate, LastPayRate, CompanyName,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.SocialSecurityNumber, o.HireDate, o.TerminationDate, o.LastPayRate, o.Company,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_ServiceHistorys o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_service_history n
    WHERE n.SocialSecurityNumber = o.SocialSecurityNumber
      AND ISNULL(n.CompanyName,'') = ISNULL(o.Company,'')
      AND ISNULL(n.HireDate,'1900-01-01') = ISNULL(o.HireDate,'1900-01-01')
  );
PRINT CONCAT('tjc_employee_service_history: inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- 8. tjc_employee_eeo (from Emp_EEO)
-----------------------------------------------------------------------------
INSERT INTO dbo.tjc_employee_eeo (
    JobGroupId,
    PopulationMale, PopulationFemale, PopulationWhite, PopulationIndian, PopulationBlack, PopulationAsian, PopulationHispanic, PopulationOther,
    HireMale, HireFemale, HireWhite, HireBlack, HireAsian, HireIndian, HireHispanic, HireOther,
    PromoMale, PromoFemale, PromoWhite, PromoBlack, PromoAsian, PromoIndian, PromoHispanic, PromoOther,
    TransferMale, TransferFemale, TransferWhite, TransferBlack, TransferAsian, TransferIndian, TransferHispanic, TransferOther,
    TermMale, TermFemale, TermWhite, TermBlack, TermIndian, TermAsian, TermHispanic, TermOther,
    Year,
    CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
SELECT
    o.JobGroup,
    o.PopulationMale, o.PopulationFemale, o.PopulationWhite, o.PopulationIndian, o.PopulationBlack, o.PopulationAsian, o.PopulationHispanic, o.PopulationOther,
    o.HireMale, o.HireFemale, o.HireWhite, o.HireBlack, o.HireAsian, o.HireIndian, o.HireHispanic, o.HireOther,
    o.PromoMale, o.PromoFemale, o.PromoWhite, o.PromoBlack, o.PromoAsian, o.PromoIndian, o.PromoHispanic, o.PromoOther,
    o.TransferMale, o.TransferFemale, o.TransferWhite, o.TransferBlack, o.TransferAsian, o.TransferIndian, o.TransferHispanic, o.TransferOther,
    o.TermMale, o.TermFemale, o.TermWhite, o.TermBlack, o.TermIndian, o.TermAsian, o.TermHispanic, o.TermOther,
    o.Year,
    GETDATE(), @SystemUser, GETDATE(), @SystemUser
FROM intranet_613.dbo.Emp_EEO o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.tjc_employee_eeo n
    WHERE n.JobGroupId = o.JobGroup AND n.Year = o.Year
  );
PRINT CONCAT('tjc_employee_eeo: inserted ', @@ROWCOUNT, ' rows');

-----------------------------------------------------------------------------
-- Post-migration counts
-----------------------------------------------------------------------------
PRINT '';
PRINT '==================== Post-migration counts ====================';
SELECT 'tjc_employee' AS TableName, COUNT(*) AS [Count] FROM dbo.tjc_employee
UNION ALL SELECT 'tjc_employee_class', COUNT(*) FROM dbo.tjc_employee_class
UNION ALL SELECT 'tjc_employee_eeo', COUNT(*) FROM dbo.tjc_employee_eeo
UNION ALL SELECT 'tjc_employee_emergency_contact', COUNT(*) FROM dbo.tjc_employee_emergency_contact
UNION ALL SELECT 'tjc_employee_group_membership', COUNT(*) FROM dbo.tjc_employee_group_membership
UNION ALL SELECT 'tjc_employee_job_group', COUNT(*) FROM dbo.tjc_employee_job_group
UNION ALL SELECT 'tjc_employee_office_location', COUNT(*) FROM dbo.tjc_employee_office_location
UNION ALL SELECT 'tjc_employee_phone', COUNT(*) FROM dbo.tjc_employee_phone
UNION ALL SELECT 'tjc_employee_position_history', COUNT(*) FROM dbo.tjc_employee_position_history
UNION ALL SELECT 'tjc_employee_race', COUNT(*) FROM dbo.tjc_employee_race
UNION ALL SELECT 'tjc_employee_service_history', COUNT(*) FROM dbo.tjc_employee_service_history
UNION ALL SELECT 'tjc_employee_assigned_item', COUNT(*) FROM dbo.tjc_employee_assigned_item
UNION ALL SELECT 'tjc_gl_group', COUNT(*) FROM dbo.tjc_gl_group
UNION ALL SELECT 'tjc_gl_counties', COUNT(*) FROM dbo.tjc_gl_counties;

COMMIT TRAN;
PRINT 'Migration complete.';
