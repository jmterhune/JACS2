/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// One month's calendar for a division. Entries and week-row headers hang off
    /// this via CalendarId. Published gates whether the public site shows it.
    /// </summary>
    [TableName("tjc_cc_calendars")]
    [PrimaryKey("CalendarId", AutoIncrement = true)]
    public class Calendar
    {
        public int CalendarId { get; set; }

        public int DivisionId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        /// <summary>Optional override for the title band (e.g. "AMENDED October 2026 Felony Div. III").</summary>
        public string TitleOverride { get; set; }

        /// <summary>Optional free-text footer / amendment note.</summary>
        public string Notes { get; set; }

        public bool Published { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public int LastModifiedByUserId { get; set; }

        public DateTime LastModifiedOnDate { get; set; }

        [IgnoreColumn]
        public string MonthName
        {
            get
            {
                if (Month < 1 || Month > 12) return string.Empty;
                return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);
            }
        }
    }
}
