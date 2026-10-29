/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// The two calendar layouts. Weekday-header renders Mon-Fri across the top
    /// with a week-type label per row (the "Mercurio" sample); row-header renders
    /// the week label in the first column (the "Felix" sample).
    /// </summary>
    public enum CalendarType
    {
        WeekdayHeader = 1,
        RowHeader = 2,
    }

    /// <summary>
    /// A court division and the standing configuration of its calendar. The
    /// assigned judge's name is denormalized onto the division so the public
    /// (external) site can display it without resolving a UserId against its own
    /// Users table - the JudgeId/judge UserId live on <see cref="Judge"/> and are
    /// meaningful only on the intranet site that authors the records.
    /// </summary>
    [TableName("tjc_cc_divisions")]
    [PrimaryKey("DivisionId", AutoIncrement = true)]
    public class Division
    {
        public int DivisionId { get; set; }

        /// <summary>e.g. "Criminal Division I", "Felony Div. III".</summary>
        public string Name { get; set; }

        public string County { get; set; }

        /// <summary>FK to tjc_cc_judges.JudgeId (0 when unassigned).</summary>
        public int JudgeId { get; set; }

        /// <summary>Display name of the assigned judge, denormalized for the public site and the picker.</summary>
        public string JudgeName { get; set; }

        /// <summary>Which layout to render; see <see cref="Components.CalendarType"/>.</summary>
        public int CalendarTypeId { get; set; }

        public string Courtroom { get; set; }

        public string ZoomMeetingId { get; set; }

        public string ZoomPassword { get; set; }

        public string JaPhone { get; set; }

        public string DivisionEmail { get; set; }

        /// <summary>URL key used by the external picker (e.g. "criminal-division-1").</summary>
        public string Slug { get; set; }

        public int SortOrder { get; set; }

        public bool Active { get; set; }

        [IgnoreColumn]
        public CalendarType LayoutType
        {
            get { return (CalendarType)CalendarTypeId; }
        }
    }
}
