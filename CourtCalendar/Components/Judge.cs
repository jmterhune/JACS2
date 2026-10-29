/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// A judge in the calendar roster and the judicial assistant assigned to help
    /// maintain their calendars. JudgeUserId and JaUserId are accounts on the
    /// intranet (apps) site - that is where judges and JAs sign in to author - and
    /// are used only there for filtering; the public site never resolves them.
    /// </summary>
    [TableName("tjc_cc_judges")]
    [PrimaryKey("JudgeId", AutoIncrement = true)]
    public class Judge
    {
        public int JudgeId { get; set; }

        public string FullName { get; set; }

        /// <summary>Intranet (apps site) UserId of the judge.</summary>
        public int JudgeUserId { get; set; }

        /// <summary>Intranet (apps site) UserId of the assigned judicial assistant (0 when none).</summary>
        public int JaUserId { get; set; }

        public string JaName { get; set; }

        public string Phone { get; set; }

        public bool Active { get; set; }
    }
}
