/*
    Court Registry admin - register the "withdrawals" review screen.

    RUN THIS AGAINST: the intranet DNN database for the environment you are deploying
    (the one that site's SiteSqlServer points at - intranet.jud12.local). This is NOT
    the database that holds the tjc_car_* tables; the withdrawal table itself is created
    by the public module's sql\deploy-withdrawals.sql against the jud12.flcourts.org
    database.

    Safe to re-run: it checks for itself first and prints what it did.

    Note: on the dev intranet site only the "Court Registry Viewer" definition is
    installed (one control, LocationList.ascx) - the admin definition with the
    Applications tab is not there, so this script will report SKIPPED on dev. That is
    expected; run it where the admin module actually lives.
*/

SET NOCOUNT ON
GO

DECLARE @ModuleDefID int, @ControlKey nvarchar(100) = N'withdrawals'

/* The admin definition is the one that owns the application detail screen. */
SELECT TOP 1 @ModuleDefID = ModuleDefID
FROM dbo.ModuleControls
WHERE ControlSrc LIKE N'%CourtRegistry/Application.ascx'

IF @ModuleDefID IS NULL
    PRINT 'SKIPPED: the Court Registry Admin module is not installed in this DNN database (no control for CourtRegistry/Application.ascx). Install the module package here, then re-run this script.'
ELSE IF EXISTS (SELECT 1 FROM dbo.ModuleControls WHERE ModuleDefID = @ModuleDefID AND ControlKey = @ControlKey)
    PRINT 'Module control "withdrawals" is already registered - left alone.'
ELSE
BEGIN
    EXEC dbo.AddModuleControl
        @ModuleDefID              = @ModuleDefID,
        @ControlKey               = @ControlKey,
        @ControlTitle             = N'Registry Withdrawals',
        @ControlSrc               = N'DesktopModules/tjc.modules/CourtRegistry/Withdrawals.ascx',
        @IconFile                 = NULL,
        @ControlType              = 0,
        @ViewOrder                = 0,
        @HelpUrl                  = NULL,
        @SupportsPartialRendering = 0,
        @SupportsPopUps           = 0,
        @CreatedByUserID          = 1

    PRINT 'Registered module control "withdrawals" on ModuleDefID ' + CAST(@ModuleDefID AS varchar(10)) + '.'
END
GO

/* What you should see: the admin definition's controls, now including withdrawals. */
SELECT mc.ModuleControlID, mc.ControlKey, mc.ControlSrc
FROM dbo.ModuleControls mc
WHERE mc.ModuleDefID IN (SELECT ModuleDefID FROM dbo.ModuleControls WHERE ControlSrc LIKE N'%CourtRegistry/Application.ascx')
ORDER BY mc.ControlKey
GO

/*
    DNN caches the module control list. Copying the new DLL into bin recycles the
    application and clears it; otherwise use Host > Clear Cache before testing.
*/
