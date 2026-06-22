/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.CDSPAdmin.Components.Models
{
    /// <summary>
    /// Read/update model for a CDSP form submission. Maps to tjc_cdsp_submission
    /// in the jud12.flcourts.org database (reached via the "Jud12" connection).
    /// Mirrors the Submission entity written by the public CDSP form module.
    /// </summary>
    [TableName("tjc_cdsp_submission")]
    [PrimaryKey("SubmissionID", AutoIncrement = true)]
    public class SubmissionInfo
    {
        public int SubmissionID { get; set; }

        // Division / County (radio selections captured by the public form)
        public int? DivisionId { get; set; }
        public string Division { get; set; }
        public int? CountyId { get; set; }
        public string County { get; set; }

        // Complainant
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        // Respondent
        public string RespondentFirstName { get; set; }
        public string RespondentLastName { get; set; }
        public string RespondentPhone { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentAddress { get; set; }

        public string Comments { get; set; }
        public bool ChildrenInvolved { get; set; }
        public string HowDidYouHear { get; set; }

        public bool Completed { get; set; }
        public int? ModuleId { get; set; }

        public int? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [IgnoreColumn]
        public string ComplainantName
        {
            get { return (((FirstName ?? string.Empty) + " " + (LastName ?? string.Empty)).Trim()); }
        }

        [IgnoreColumn]
        public string RespondentName
        {
            get { return (((RespondentFirstName ?? string.Empty) + " " + (RespondentLastName ?? string.Empty)).Trim()); }
        }
    }
}
