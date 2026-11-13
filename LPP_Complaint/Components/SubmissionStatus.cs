/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System.Collections.Generic;

namespace tjc.Modules.LPP_Complaint.Components
{
    /// <summary>
    /// The handling states a submitted complaint moves through. Stored as the text
    /// itself rather than a code, so a row still reads plainly in a SQL query or an
    /// exported spreadsheet.
    /// </summary>
    public static class SubmissionStatus
    {
        public const string New = "New";
        public const string UnderReview = "Under Review";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";

        public static IList<string> All
        {
            get { return new List<string> { New, UnderReview, Resolved, Closed }; }
        }

        /// <summary>A bootstrap badge class, so the list reads at a glance.</summary>
        public static string BadgeClass(string status)
        {
            switch (status)
            {
                case UnderReview: return "bg-info text-dark";
                case Resolved: return "bg-success";
                case Closed: return "bg-secondary";
                default: return "bg-warning text-dark";
            }
        }
    }
}
