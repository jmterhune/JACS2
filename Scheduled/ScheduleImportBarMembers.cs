using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using GenericParsing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using tjc.Modules.jacs.Components;
using WinSCP;

namespace tjc.Modules.jacs.Scheduled
{
    /// <summary>
    /// Daily SchedulerClient that downloads the Florida Bar member file from
    /// the SFTP drop and inserts any *eligible* attorneys that JACS does not
    /// already have a row for. Matches the standalone "Bar Member Import"
    /// service for the SFTP + parse steps, but writes directly to the JACS
    /// attorneys table instead of into a staging table.
    ///
    /// SFTP credentials, file names, and the local extract path live in
    /// web.config appSettings under the JACS.BarImport.* prefix. CSV parsing
    /// helpers + the eligibility code are in <see cref="BarMemberFile"/>.
    /// </summary>
    public class ScheduleImportBarMembers : SchedulerClient
    {
        public ScheduleImportBarMembers(ScheduleHistoryItem scheduleHistoryItem)
        {
            ScheduleHistoryItem = scheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                ScheduleHistoryItem.AddLogNote("Starting bar member import...");

                // Every fatal condition (missing config, FTP/CSV failure)
                // throws so the catch block below calls Errored(ref ex) +
                // LogException uniformly. DNN treats Succeeded=false WITHOUT
                // Errored() as "ran but didn't succeed" — the task list will
                // not surface it as an error, which masked FTP connect
                // failures in earlier versions.
                var cfg = BarMemberFile.LoadConfig(out string configError);
                if (cfg == null)
                    throw new InvalidOperationException(configError);

                Directory.CreateDirectory(cfg.LocalDestinationPath);
                string zipPath = Path.Combine(cfg.LocalDestinationPath, cfg.BarFileName);
                string csvPath = Path.Combine(cfg.LocalDestinationPath, cfg.CsvFileName);

                if (!Download(cfg, zipPath))
                    throw new InvalidOperationException(
                        $"SFTP download failed after {cfg.MaxRetries} attempt(s) " +
                        $"(host {cfg.Host}:{cfg.Port}, file {cfg.BarFileName}). " +
                        "See preceding log notes for per-attempt detail.");

                if (File.Exists(csvPath)) File.Delete(csvPath);
                ZipFile.ExtractToDirectory(zipPath, cfg.LocalDestinationPath);

                if (!File.Exists(csvPath))
                    throw new InvalidOperationException(
                        $"Expected CSV not found after extract: {csvPath}.");

                var stats = ImportFromCsv(csvPath);

                ScheduleHistoryItem.AddLogNote(
                    $"Bar member import complete. Rows: {stats.Total}, " +
                    $"ineligible: {stats.Ineligible}, " +
                    $"missing bar_num: {stats.NoBarNum}, " +
                    $"attorneys inserted: {stats.AttorneyInserted}, " +
                    $"attorneys already present: {stats.AttorneyAlreadyPresent}, " +
                    $"emails inserted: {stats.EmailInserted}, " +
                    $"emails already present: {stats.EmailAlreadyPresent}, " +
                    $"errors: {stats.Errored}.");

                if (stats.Errored > 0)
                    throw new InvalidOperationException(
                        $"{stats.Errored} row(s) failed during import. Check the JACS event log for individual exceptions.");

                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote($"Bar member import errored: {ex.Message}");
                Errored(ref ex);
                Exceptions.LogException(ex);
            }
        }

        // ---------------------------------------------------------------------
        // SFTP download
        // ---------------------------------------------------------------------

        private bool Download(BarImportConfig cfg, string localZipPath)
        {
            // Yesterday's day-of-week is the remote folder name.
            string fileDay = DateTime.Today.AddDays(-1).ToString("dddd");
            string remoteFile = $"/{fileDay}/{cfg.BarFileName}";

            // Log every parameter we're about to use so a connection failure
            // tells the operator which host/port/user was attempted — the
            // WinSCP exception message ("Software caused connection abort"
            // etc.) gives no context on its own.
            ScheduleHistoryItem.AddLogNote(
                $"Opening SFTP session to {cfg.Host}:{cfg.Port} as '{cfg.Username}' (remote {remoteFile}).");

            var options = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = cfg.Host,
                PortNumber = cfg.Port,
                UserName = cfg.Username,
                Password = cfg.Password,
                // Match the standalone service: bar drop server's host key is
                // not pinned, so accept any. This is a private vendor SFTP, not
                // a public network resource.
                SshHostKeyPolicy = SshHostKeyPolicy.GiveUpSecurityAndAcceptAny,
                // Bound the connection attempt so an unreachable host fails
                // the run instead of hanging indefinitely.
                Timeout = TimeSpan.FromSeconds(30),
            };

            if (File.Exists(localZipPath)) File.Delete(localZipPath);

            using (var session = new Session())
            {
                session.Open(options);

                for (int attempt = 1; attempt <= cfg.MaxRetries; attempt++)
                {
                    if (!session.FileExists(remoteFile))
                    {
                        ScheduleHistoryItem.AddLogNote(
                            $"Remote file not found: {remoteFile} (attempt {attempt}/{cfg.MaxRetries}).");
                        continue;
                    }

                    var transferOptions = new TransferOptions { TransferMode = TransferMode.Binary };
                    var result = session.GetFiles(remoteFile, localZipPath, false, transferOptions);
                    result.Check();

                    if (File.Exists(localZipPath))
                    {
                        ScheduleHistoryItem.AddLogNote(
                            $"Downloaded {remoteFile} to {localZipPath} (attempt {attempt}).");
                        return true;
                    }
                }
            }

            return false;
        }

        // ---------------------------------------------------------------------
        // CSV parse + import
        // ---------------------------------------------------------------------

        private ImportStats ImportFromCsv(string csvPath)
        {
            var stats = new ImportStats();

            DataTable rows;
            using (var parser = new GenericParserAdapter(csvPath))
            {
                parser.ColumnDelimiter = ',';
                parser.FirstRowHasHeader = false;
                parser.TextQualifier = '"';
                rows = parser.GetDataTable();
            }

            // Pre-fetch every existing attorney (bar_num -> id) and every
            // email address already on file. Both are in-memory lookups so
            // the per-row checks are O(1) instead of one SELECT per CSV row
            // (the file runs to 100k+ rows).
            var attCtl = new AttorneyController();
            var emailCtl = new EmailController();
            var existingAttorneys = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
            foreach (var a in attCtl.GetAttorneys())
            {
                string norm = BarMemberFile.NormalizeBarNum(a.bar_num);
                if (!string.IsNullOrEmpty(norm) && !existingAttorneys.ContainsKey(norm))
                    existingAttorneys[norm] = a.id;
            }
            var existingEmails = new HashSet<string>(
                emailCtl.GetAllEmailAddresses()
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(e => e.Trim()),
                StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in rows.Rows)
            {
                stats.Total++;
                try
                {
                    string status = BarMemberFile.CellOrEmpty(row, BarMemberFile.ColStatus);
                    if (!string.Equals(status, BarMemberFile.EligibleStatus, StringComparison.OrdinalIgnoreCase))
                    {
                        stats.Ineligible++;
                        continue;
                    }

                    string barNum = BarMemberFile.NormalizeBarNum(
                        BarMemberFile.CellOrEmpty(row, BarMemberFile.ColBarNum));
                    if (string.IsNullOrEmpty(barNum))
                    {
                        stats.NoBarNum++;
                        continue;
                    }

                    // Insert attorney if missing; otherwise reuse the existing id.
                    long attorneyId;
                    if (existingAttorneys.TryGetValue(barNum, out attorneyId))
                    {
                        stats.AttorneyAlreadyPresent++;
                    }
                    else
                    {
                        var attorney = BuildAttorneyRow(row, barNum);
                        attCtl.CreateAttorney(attorney);
                        attorneyId = attorney.id;
                        existingAttorneys[barNum] = attorneyId;
                        stats.AttorneyInserted++;
                    }

                    // Email: insert only if this address isn't anywhere in
                    // emails yet (global dedup, per the import policy). Covers
                    // both newly-inserted attorneys and existing ones whose
                    // bar-side email wasn't on file in JACS.
                    string email = BarMemberFile.CellOrEmpty(row, BarMemberFile.ColEmail);
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        string trimmed = email.Trim();
                        if (!existingEmails.Contains(trimmed))
                        {
                            emailCtl.CreateEmail(new Email
                            {
                                emailable_id = attorneyId,
                                emailable_type = "App\\Models\\Attorney",
                                email = trimmed,
                            });
                            existingEmails.Add(trimmed);
                            stats.EmailInserted++;
                        }
                        else
                        {
                            stats.EmailAlreadyPresent++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    stats.Errored++;
                    // One bad row should not abort the whole run.
                    Exceptions.LogException(new Exception(
                        $"ScheduleImportBarMembers: row {stats.Total} failed: {ex.Message}", ex));
                }
            }

            return stats;
        }

        // Build an Attorney row WITHOUT the emails collection — emails are
        // handled separately (dedup-by-address) after the attorney is
        // inserted, so CreateAttorney shouldn't blindly insert a duplicate.
        private static Attorney BuildAttorneyRow(DataRow row, string barNum)
        {
            string first = BarMemberFile.CellOrEmpty(row, BarMemberFile.ColFirstName);
            string middle = BarMemberFile.CellOrEmpty(row, BarMemberFile.ColMiddleName);
            string last = BarMemberFile.CellOrEmpty(row, BarMemberFile.ColLastName);
            string suffix = BarMemberFile.CellOrEmpty(row, BarMemberFile.ColSuffix);

            string name = BarMemberFile.JoinNameFirstMiddleLast(first, middle, last, suffix);
            if (string.IsNullOrWhiteSpace(name)) name = barNum;

            string phone = BarMemberFile.FormatPhone(
                BarMemberFile.CellOrEmpty(row, BarMemberFile.ColBusAreaCode),
                BarMemberFile.CellOrEmpty(row, BarMemberFile.ColBusPhone));

            return new Attorney
            {
                UserId = 0,
                name = name,
                bar_num = barNum,
                phone = phone,
                scheduling = false,
                enabled = true,
                emails = null,
            };
        }

        private class ImportStats
        {
            public int Total;
            public int Ineligible;
            public int NoBarNum;
            public int AttorneyInserted;
            public int AttorneyAlreadyPresent;
            public int EmailInserted;
            public int EmailAlreadyPresent;
            public int Errored;
        }
    }
}
