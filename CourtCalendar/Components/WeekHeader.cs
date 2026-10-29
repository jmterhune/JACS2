/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// A row label for one week of a calendar - "Week 5" / "Week 6" on the
    /// row-header layout, or a week-type label such as "Trial Week" /
    /// "Administrative Week" on the weekday-header layout. WeekStartDate is the
    /// Monday of the week it labels.
    /// </summary>
    [TableName("tjc_cc_week_headers")]
    [PrimaryKey("WeekHeaderId", AutoIncrement = true)]
    public class WeekHeader
    {
        public int WeekHeaderId { get; set; }

        public int CalendarId { get; set; }

        public DateTime WeekStartDate { get; set; }

        public string Label { get; set; }

        /// <summary>Optional second line (e.g. "(Sept. 30)", "FIRST APPEARANCES").</summary>
        public string SubLabel { get; set; }

        public int SortOrder { get; set; }
    }
}
