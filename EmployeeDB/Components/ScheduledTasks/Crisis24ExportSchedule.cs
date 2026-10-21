using DotNetNuke.Services.Scheduling;
using System;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Components.ScheduledTasks
{
    /// <summary>
    /// Runs the Crisis24 Person/HR feed export automatically, so HR doesn't
    /// have to remember to click "Crisis24 Export" on EmployeeList every
    /// month. Registered as a monthly DNN scheduled task by
    /// Providers/DataProviders/SqlDataProvider/00.00.05.SqlDataProvider.
    ///
    /// Builds and uploads the exact same file the manual button sends — see
    /// Components/Api/Crisis24Controller.cs::Export() for the interactive
    /// version of this same logic (the one difference: this reports success
    /// or failure into ScheduleHistoryItem instead of an HTTP response, since
    /// there's no page or JS layer to show a result to).
    ///
    /// Scheduled tasks run with no HttpContext, so unlike Crisis24Controller
    /// this can't resolve IHostSettings from the per-request DI scope.
    /// Crisis24ExportBuilder.Build(null) is fine here — the controllers it
    /// calls (EmployeeReportController, PhoneController) fall back to the
    /// classic DataContext.Instance() overload when hostSettings is null.
    /// </summary>
    public class Crisis24ExportSchedule : SchedulerClient
    {
        public Crisis24ExportSchedule(ScheduleHistoryItem oItem) : base()
        {
            ScheduleHistoryItem = oItem;
        }

        public override void DoWork()
        {
            try
            {
                Progressing();
                ScheduleHistoryItem.AddLogNote("Starting Crisis24 Person/HR feed export\n");

                if (!Crisis24Uploader.IsConfigured)
                {
                    // Matches the manual button's "not set up yet" guard —
                    // dev/test sites without a real Crisis24FTP destination
                    // shouldn't fail (and alert on) every scheduled run.
                    ScheduleHistoryItem.AddLogNote("Crisis24FTP is not configured on this site — skipping.\n");
                    ScheduleHistoryItem.Succeeded = true;
                    return;
                }

                var file = Crisis24ExportBuilder.Build(null);

                if (file.RowCount == 0)
                {
                    // A full file is expected on every transmission, so an
                    // empty roster is a data problem, not a valid feed —
                    // sending it would look like a mass termination. Matches
                    // the same guard on the manual Export button.
                    ScheduleHistoryItem.AddLogNote("No employees were found, so nothing was sent to Crisis24.\n");
                    ScheduleHistoryItem.Succeeded = true;
                    return;
                }

                var upload = Crisis24Uploader.Upload(file.Content, file.FileName);

                // Only a real, successful upload moves the "last export"
                // watermark forward, so already-reported terminations keep
                // dropping off subsequent runs. userId -1: no logged-in user
                // to attribute a scheduled run to.
                new Crisis24ExportLogController(null).RecordExport(file.FileName, file.RowCount);

                ScheduleHistoryItem.AddLogNote(
                    file.RowCount + " employee record(s) sent to Crisis24 as " + file.FileName
                    + " (" + upload.ByteCount + " bytes to " + upload.Host + upload.RemotePath + ")\n");
                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote("Error performing Crisis24 export: " + ex.Message + "\n");
                Errored(ref ex);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
        }
    }
}
