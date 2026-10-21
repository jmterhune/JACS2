using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Data;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    /// <summary>
    /// Tracks successful Crisis24 Person/HR feed transmissions in
    /// tjc_crisis24_export_log, so Crisis24ExportBuilder can tell which
    /// inactive employees have already been reported (and can drop off the
    /// feed) versus which were terminated since the last successful upload
    /// (and still need to go out as "I"). A row is recorded only after a real
    /// upload succeeds — building a Preview, or a failed upload, must not
    /// move this window forward.
    /// </summary>
    public class Crisis24ExportLogController
    {
        private readonly IHostSettings _hostSettings;

        public Crisis24ExportLogController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        /// <summary>Timestamp of the most recent successful export, or null
        /// if none has ever succeeded. Also called from the monthly
        /// Crisis24ExportSchedule task, which has no HttpContext to resolve
        /// an IHostSettings from — see EmployeeReportController for the same
        /// null-hostSettings fallback pattern.</summary>
        public DateTime? GetLastSuccessfulExportDate()
        {
            using (IDataContext ctx = _hostSettings != null ? DataContext.Instance(_hostSettings) : DataContext.Instance())
            {
                return ctx.ExecuteScalar<DateTime?>(CommandType.Text,
                    "SELECT MAX(CreatedDate) FROM tjc_crisis24_export_log WHERE Success = 1");
            }
        }

        /// <summary>Records a successful transmission. userId is -1 when
        /// called from the scheduled task (no logged-in user to attribute it
        /// to), matching the sentinel used elsewhere in this module.</summary>
        public void RecordExport(string fileName, int rowCount, int userId = -1)
        {
            using (IDataContext ctx = _hostSettings != null ? DataContext.Instance(_hostSettings) : DataContext.Instance())
            {
                string sql = @"INSERT INTO tjc_crisis24_export_log
                                   (FileName, RecordCount, Success, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById)
                               VALUES (@0, @1, 1, @2, @3, @2, @3)";
                ctx.Execute(CommandType.Text, sql, fileName, rowCount, DateTime.Now, userId);
            }
        }
    }
}
