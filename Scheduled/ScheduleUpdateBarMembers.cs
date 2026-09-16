using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using GenericParsing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Scheduled
{
    /// <summary>
    /// Companion to <see cref="ScheduleImportBarMembers"/>. Refreshes the
    /// fields on attorneys that JACS already knows about, using the most
    /// recently downloaded Florida Bar member CSV at
    /// <c>JACS.BarImport.LocalDestinationPath</c>. Does NOT re-download from
    /// SFTP — the import scheduler is the only thing that talks to the
    /// vendor; this one just re-reads what's on disk.
    ///
    /// For each CSV row whose bar number matches an existing attorney, the
    /// following are refreshed:
    ///   - <c>name</c>          → "Last, First Middle [Suffix]"
    ///   - <c>phone</c>         → "(AAA) XXX-XXXX" from business area code + phone
    ///   - <c>enabled</c>       → true iff the CSV's status is "Y" (eligible)
    /// Rows whose bar number isn't in the attorneys table are skipped (the
    /// import scheduler is responsible for adding them).
    ///
    /// Email handling matches the import: if the CSV email is not already
    /// present in the emails table (matched by address, regardless of which
    /// attorney owns it), add a new row with
    /// <c>emailable_type='App\Models\Attorney'</c> tied to this attorney.
    /// </summary>
    public class ScheduleUpdateBarMembers : SchedulerClient
    {
        public ScheduleUpdateBarMembers(ScheduleHistoryItem scheduleHistoryItem)
        {
            ScheduleHistoryItem = scheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                ScheduleHistoryItem.AddLogNote("Starting bar member update...");

                // Fatal conditions throw so the catch block calls
                // Errored(ref ex) + LogException uniformly. Mirrors
                // ScheduleImportBarMembers's failure-flow policy: anything
                // that prevents the task from completing must surface in DNN
                // as an errored run, not a soft Succeeded=false.
                var cfg = BarMemberFile.LoadConfig(out string configError);
                if (cfg == null)
                    throw new InvalidOperationException(configError);

                string csvPath = Path.Combine(cfg.LocalDestinationPath, cfg.CsvFileName);
                if (!File.Exists(csvPath))
                    throw new InvalidOperationException(
                        $"CSV not found at {csvPath}. The import scheduler must run first " +
                        "to download and extract the file.");

                var stats = UpdateFromCsv(csvPath);

                ScheduleHistoryItem.AddLogNote(
                    $"Bar member update complete. Rows: {stats.Total}, " +
                    $"missing bar_num: {stats.NoBarNum}, " +
                    $"attorney not in JACS: {stats.NoMatch}, " +
                    $"attorneys updated: {stats.AttorneyUpdated}, " +
                    $"attorneys unchanged: {stats.AttorneyUnchanged}, " +
                    $"emails inserted: {stats.EmailInserted}, " +
                    $"emails already present: {stats.EmailAlreadyPresent}, " +
                    $"errors: {stats.Errored}.");

                if (stats.Errored > 0)
                    throw new InvalidOperationException(
                        $"{stats.Errored} row(s) failed during update. Check the JACS event log for individual exceptions.");

                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote($"Bar member update errored: {ex.Message}");
                Errored(ref ex);
                Exceptions.LogException(ex);
            }
        }

        private UpdateStats UpdateFromCsv(string csvPath)
        {
            var stats = new UpdateStats();

            DataTable rows;
            using (var parser = new GenericParserAdapter(csvPath))
            {
                parser.ColumnDelimiter = ',';
                parser.FirstRowHasHeader = false;
                parser.TextQualifier = '"';
                rows = parser.GetDataTable();
            }

            // Pre-fetch attorneys (by bar_num) and email addresses so the
            // per-row lookups are O(1). Same scaling problem as the import:
            // the file runs to 100k+ rows.
            var attCtl = new AttorneyController();
            var emailCtl = new EmailController();

            var attorneysByBarNum = new Dictionary<string, Attorney>(StringComparer.OrdinalIgnoreCase);
            foreach (var a in attCtl.GetAttorneys())
            {
                string norm = BarMemberFile.NormalizeBarNum(a.bar_num);
                if (!string.IsNullOrEmpty(norm) && !attorneysByBarNum.ContainsKey(norm))
                    attorneysByBarNum[norm] = a;
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
                    string barNum = BarMemberFile.NormalizeBarNum(
                        BarMemberFile.CellOrEmpty(row, BarMemberFile.ColBarNum));
                    if (string.IsNullOrEmpty(barNum))
                    {
                        stats.NoBarNum++;
                        continue;
                    }

                    if (!attorneysByBarNum.TryGetValue(barNum, out var attorney))
                    {
                        // No existing JACS row to update; the import scheduler
                        // handles adding new attorneys (and only those that
                        // are eligible). Updates intentionally do nothing.
                        stats.NoMatch++;
                        continue;
                    }

                    bool changed = ApplyUpdates(row, attorney);
                    if (changed)
                    {
                        attCtl.UpdateAttorney(attorney);
                        stats.AttorneyUpdated++;
                    }
                    else
                    {
                        stats.AttorneyUnchanged++;
                    }

                    // Email: dedup by address. Same rule as the import — only
                    // insert when the address isn't already on file anywhere.
                    string email = BarMemberFile.CellOrEmpty(row, BarMemberFile.ColEmail);
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        string trimmed = email.Trim();
                        if (!existingEmails.Contains(trimmed))
                        {
                            emailCtl.CreateEmail(new Email
                            {
                                emailable_id = attorney.id,
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
                    Exceptions.LogException(new Exception(
                        $"ScheduleUpdateBarMembers: row {stats.Total} failed: {ex.Message}", ex));
                }
            }

            return stats;
        }

        /// <summary>
        /// Mutates the given attorney's name/phone/enabled fields from this
        /// CSV row. Returns true when at least one field actually changed —
        /// the caller uses that to skip an UPDATE round-trip when there is
        /// nothing to write.
        ///
        /// Name and phone are only overwritten when the CSV produced a
        /// non-empty replacement; an empty bar-file value never clobbers
        /// data already on the JACS row. Eligibility (enabled) is always
        /// synchronised so suspensions/disbarments propagate.
        /// </summary>
        private static bool ApplyUpdates(DataRow row, Attorney attorney)
        {
            string newName = BarMemberFile.JoinNameLastFirstMiddle(
                BarMemberFile.CellOrEmpty(row, BarMemberFile.ColFirstName),
                BarMemberFile.CellOrEmpty(row, BarMemberFile.ColMiddleName),
                BarMemberFile.CellOrEmpty(row, BarMemberFile.ColLastName),
                BarMemberFile.CellOrEmpty(row, BarMemberFile.ColSuffix));

            string newPhone = BarMemberFile.FormatPhone(
                BarMemberFile.CellOrEmpty(row, BarMemberFile.ColBusAreaCode),
                BarMemberFile.CellOrEmpty(row, BarMemberFile.ColBusPhone));

            string status = BarMemberFile.CellOrEmpty(row, BarMemberFile.ColStatus);
            bool newEnabled = string.Equals(status, BarMemberFile.EligibleStatus, StringComparison.OrdinalIgnoreCase);

            bool changed = false;

            if (!string.IsNullOrWhiteSpace(newName) && !string.Equals(newName, attorney.name, StringComparison.Ordinal))
            {
                attorney.name = newName;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(newPhone) && !string.Equals(newPhone, attorney.phone, StringComparison.Ordinal))
            {
                attorney.phone = newPhone;
                changed = true;
            }

            if (!attorney.enabled.HasValue || attorney.enabled.Value != newEnabled)
            {
                attorney.enabled = newEnabled;
                changed = true;
            }

            return changed;
        }

        private class UpdateStats
        {
            public int Total;
            public int NoBarNum;
            public int NoMatch;
            public int AttorneyUpdated;
            public int AttorneyUnchanged;
            public int EmailInserted;
            public int EmailAlreadyPresent;
            public int Errored;
        }
    }
}
