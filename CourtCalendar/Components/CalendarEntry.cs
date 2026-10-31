/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// One scheduled item in a day cell. The public calendar shows Timespan +
    /// ShortTitle in the cell and reveals Detail in a hover popover. A stricken
    /// entry stays visible but is rendered struck-through with a "cancelled"
    /// indicator.
    /// </summary>
    [TableName("tjc_cc_entries")]
    [PrimaryKey("EntryId", AutoIncrement = true)]
    public class CalendarEntry
    {
        public int EntryId { get; set; }

        public int CalendarId { get; set; }

        public DateTime EntryDate { get; set; }

        /// <summary>e.g. "9:00", "9:00-5:00", "1:30". Ignored when <see cref="AllDay"/> is true.</summary>
        public string Timespan { get; set; }

        /// <summary>When true the entry has no time; the cell shows only the short title.</summary>
        public bool AllDay { get; set; }

        /// <summary>e.g. "Pleas/Motions", "Trials (Backup)".</summary>
        public string ShortTitle { get; set; }

        /// <summary>Render the short title bold.</summary>
        public bool TitleBold { get; set; }

        /// <summary>Render the short title italic.</summary>
        public bool TitleItalic { get; set; }

        /// <summary>Popover detail (e.g. "SPECIAL SET", "BARTHOLOMEW", "(Ibarra/Callin)").</summary>
        public string Detail { get; set; }

        /// <summary>FK to tjc_cc_categories.CategoryId; 0 means the division's default category (no color).</summary>
        public int CategoryId { get; set; }

        public int SortOrder { get; set; }

        public bool IsStricken { get; set; }

        public int StrickenByUserId { get; set; }

        public DateTime? StrickenOnDate { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedOnDate { get; set; }
    }
}
