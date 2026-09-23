/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace tjc.Modules.Arbitrators.Components
{
    public enum ApplicationStatus
    {
        Pending = 0,
        Approved = 1,
        Denied = 2,
        Removed = 3
    }

    /// <summary>
    /// Read/update model for an arbitrator application. Maps to
    /// tjc_arbitrators_applications in the jud12.flcourts.org database (reached
    /// via the "Jud12" connection). Mirrors the entity written by the public
    /// Arbitrators module (tjc.Modules\Arbitrators\Components\ArbitratorApplication.cs) —
    /// this is the internal review sister to that module, so the schema must stay
    /// in sync with it.
    /// </summary>
    [TableName("tjc_arbitrators_applications")]
    [PrimaryKey("ArbitratorApplicationID", AutoIncrement = true)]
    public class ArbitratorApplication
    {
        public int ArbitratorApplicationID { get; set; }

        /// <summary>
        /// The DNN account (on the jud12.flcourts.org portal) that owns this
        /// application. Used to look up that account's verified email for
        /// applicant-facing notifications, rather than the free-text business
        /// Email field below.
        /// </summary>
        public int UserId { get; set; }

        public int CreatedByUserId { get; set; }
        public int LastModifiedByUserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string PreferredAreas { get; set; }
        public bool FloridaBarMember { get; set; }
        public bool CertifiedMediator { get; set; }
        public bool CompletedArbitrationTraining { get; set; }
        public string ExperienceStatement { get; set; }
        public string SignedName { get; set; }
        public DateTime SignatureDate { get; set; }
        public string SubmittedIP { get; set; }
        public ApplicationStatus Status { get; set; }
        public string StatusNotes { get; set; }
        public int? ReviewedByUserId { get; set; }
        public DateTime? ReviewedOnDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime LastModifiedOnDate { get; set; }

        /// <summary>Display name for the review list and notification emails.</summary>
        [IgnoreColumn]
        public string FullName
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(FirstName)) parts.Add(FirstName);
                if (!string.IsNullOrWhiteSpace(MiddleName)) parts.Add(MiddleName);
                if (!string.IsNullOrWhiteSpace(LastName)) parts.Add(LastName);
                string name = string.Join(" ", parts);
                if (!string.IsNullOrWhiteSpace(Suffix)) name += ", " + Suffix;
                return name;
            }
        }
    }
}
