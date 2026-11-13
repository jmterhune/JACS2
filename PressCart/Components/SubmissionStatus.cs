/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System.Collections.Generic;

namespace tjc.Modules.PressCart.Components
{
    /// <summary>
    /// The handling states a submitted cart request moves through. Stored as the text
    /// itself rather than a code, so a row still reads plainly in a SQL query or an
    /// exported spreadsheet.
    /// </summary>
    public static class SubmissionStatus
    {
        public const string New = "New";
        public const string Reserved = "Reserved";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static IList<string> All
        {
            get { return new List<string> { New, Reserved, Completed, Cancelled }; }
        }

        /// <summary>A bootstrap badge class, so the list reads at a glance.</summary>
        public static string BadgeClass(string status)
        {
            switch (status)
            {
                case Reserved: return "bg-info text-dark";
                case Completed: return "bg-success";
                case Cancelled: return "bg-secondary";
                default: return "bg-warning text-dark";
            }
        }
    }
}
