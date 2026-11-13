/*
	Module icons for the intranet (apps) site
	=========================================

	Run against the DNN database of the INTRANET site.

	Four modules named an icon that is not on disk, so DNN showed a broken
	image wherever the module is listed - Host > Extensions, and the module
	picker when adding one to a page:

		Court Calendar        Calendar_32X32_Standard.png   - no such file
		Interpreter Request   Comments_32X32_Standard.png   - no such file
		Press Cart            Monitor_32X32_Standard.png    - no such file
		Threat Report         a stale 1.1.0 path into a DesktopModules
		                      \Images folder that no longer exists

	LPP Complaint named Lock_32X32_Standard.png where the file is
	Lock_32x32_Standard.png. That one renders today only because Windows is
	case-insensitive; the casing is corrected here so it keeps working if the
	site is ever served from a case-sensitive filesystem.

	Each replacement is a file that ships with DNN in the site's Icons\Sigma
	folder. The matching .dnn manifests were fixed in the same change, so a
	fresh install of any of these packages already carries the right value -
	this script is only for sites where the package is ALREADY installed.

	Idempotent: it touches only rows that exist and are not already correct.
	Applied to dev and test on 2026-11-13.
*/

SET NOCOUNT ON;
GO

/*--------------------------------------------------------------------------
  Guard.

  SET @ExpectedDb BEFORE RUNNING. There is no safe default: the intranet
  database is named differently per environment (dev and test are both
  'intranet.jud12.local'), and several of these package names exist on the
  public site's database too - Threat Report, for one - so the script cannot
  work out on its own which database it is looking at.

  Two further checks run after the name: the database must be a DNN
  installation, and it must NOT be the public site's database, which is
  recognised by the tjc_pressCart table.
--------------------------------------------------------------------------*/

DECLARE @ExpectedDb nvarchar(128) = NULL;   -- e.g. N'intranet.jud12.local'

IF @ExpectedDb IS NULL
BEGIN
	RAISERROR (N'Set @ExpectedDb to the intranet database name for this environment before running. Nothing has been changed.', 16, 1);
	-- Suppresses every statement that follows, on this connection, until
	-- SET NOEXEC OFF at the end. Nothing below runs against the wrong database.
	SET NOEXEC ON;
END
ELSE IF DB_NAME() <> @ExpectedDb
BEGIN
	DECLARE @wrongDb nvarchar(128) = DB_NAME();
	RAISERROR (N'This script expects the %s database. Current database: %s', 16, 1, @ExpectedDb, @wrongDb);
	SET NOEXEC ON;
END
GO

IF OBJECT_ID(N'dbo.Packages') IS NULL
BEGIN
	RAISERROR (N'This is not a DNN database (no dbo.Packages table). Nothing has been changed.', 16, 1);
	SET NOEXEC ON;
END
GO

IF OBJECT_ID(N'dbo.tjc_pressCart') IS NOT NULL
BEGIN
	RAISERROR (N'This is the PUBLIC site database (it has tjc_pressCart), not the intranet one. Nothing has been changed.', 16, 1);
	SET NOEXEC ON;
END
GO


/*==========================================================================
  1.  Before
==========================================================================*/

PRINT '--- icon paths before ---';

SELECT PackageID, Name, FriendlyName, IconFile
FROM dbo.Packages
WHERE Name IN (N'CourtCalendarAdmin', N'InterpreterRequestAdmin', N'PressCartAdmin',
               N'LPP_ComplaintAdmin', N'ThreatReport')
ORDER BY Name;
GO


/*==========================================================================
  2.  Correct the icon paths

  A package that is not installed on this site simply matches no row, so the
  statement updates nothing and the script carries on.
==========================================================================*/

DECLARE @icons TABLE (PackageName nvarchar(128) NOT NULL PRIMARY KEY, IconFile nvarchar(255) NOT NULL);

INSERT INTO @icons (PackageName, IconFile)
VALUES (N'CourtCalendarAdmin',      N'~/Icons/Sigma/ScheduleHistory_32x32_Standard.png'),
       (N'InterpreterRequestAdmin', N'~/Icons/Sigma/Translate_32x32_Standard.png'),
       (N'PressCartAdmin',          N'~/Icons/Sigma/Console_32x32_Standard.png'),
       (N'LPP_ComplaintAdmin',      N'~/Icons/Sigma/Lock_32x32_Standard.png'),
       (N'ThreatReport',            N'~/Icons/Sigma/SecurityRoles_32x32_Standard.png');

UPDATE p
SET    p.IconFile = i.IconFile
FROM   dbo.Packages p
JOIN   @icons i ON i.PackageName = p.Name
-- BINARY collation, so a row differing only in the case of the x in 32x32 is
-- still corrected.
WHERE  ISNULL(p.IconFile, N'') COLLATE Latin1_General_BIN <> i.IconFile COLLATE Latin1_General_BIN;

PRINT 'Updated ' + CONVERT(nvarchar(10), @@ROWCOUNT) + ' package icon path(s).';
GO


/*==========================================================================
  3.  After
==========================================================================*/

PRINT '--- icon paths after ---';

SELECT PackageID, Name, FriendlyName, IconFile
FROM dbo.Packages
WHERE Name IN (N'CourtCalendarAdmin', N'InterpreterRequestAdmin', N'PressCartAdmin',
               N'LPP_ComplaintAdmin', N'ThreatReport')
ORDER BY Name;
GO

/*
	Afterwards:

	  - DNN caches package and module data, so recycle the application pool or
	    clear the cache from Host > Server Settings before checking.
	  - Confirm the files are present in the site's Icons\Sigma folder. They
	    ship with DNN, so they should be, but a trimmed deployment may not have
	    them:
	        ScheduleHistory_32x32_Standard.png
	        Translate_32x32_Standard.png
	        Console_32x32_Standard.png
	        Lock_32x32_Standard.png
	        SecurityRoles_32x32_Standard.png
*/

SET NOEXEC OFF;
GO
