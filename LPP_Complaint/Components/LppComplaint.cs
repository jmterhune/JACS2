/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace tjc.Modules.LPP_Complaint.Components
{
    /// <summary>
    /// One Local Professionalism Panel complaint submitted on the public site. The rows
    /// live in the public (jud12) database and are reached from here through the "Jud12"
    /// connection, so the record is deliberately not scoped to ModuleId: the ModuleId on
    /// it belongs to the public site's form module, not to anything on this intranet site.
    /// </summary>
    [TableName("tjc_modules_lpp_complaints")]
    [PrimaryKey("LppComplaintId", AutoIncrement = true)]
    public class LppComplaint
    {
        public int LppComplaintId { get; set; }

        public int ModuleId { get; set; }

        public int PortalId { get; set; }

        public DateTime CreatedOnDate { get; set; }

        // ---- Referring party ----

        public string ReferrerName { get; set; }

        public string ReferrerEmail { get; set; }

        public string ReferrerAddress { get; set; }

        public string ReferrerCity { get; set; }

        public string ReferrerState { get; set; }

        public string ReferrerZip { get; set; }

        public string ReferrerPhone { get; set; }

        /// <summary>Nature of the referrer's relationship with the attorney being referred.</summary>
        public string Relationship { get; set; }

        // ---- Attorney being referred ----

        public string AttorneyName { get; set; }

        public string AttorneyBarNumber { get; set; }

        public string AttorneyAddress { get; set; }

        public string AttorneyCity { get; set; }

        public string AttorneyState { get; set; }

        public string AttorneyZip { get; set; }

        public string AttorneyPhone { get; set; }

        public string AttorneyEmail { get; set; }

        // ---- Alleged violation ----

        /// <summary>Kept as typed: the form takes free text through a date picker.</summary>
        public string ViolationDate { get; set; }

        public string County { get; set; }

        public string IncidentDescription { get; set; }

        /// <summary>The typed name the form treats as an electronic signature.</summary>
        public string SignedBy { get; set; }

        // ---- Handling, set from the intranet administration module ----

        public string Status { get; set; }

        public string StatusNotes { get; set; }

        public int? StatusChangedByUserId { get; set; }

        public DateTime? StatusChangedOnDate { get; set; }

        /// <summary>
        /// The ticked "Nature of Incident" boxes, loaded separately from
        /// tjc_modules_lpp_complaint_natures. Never a column on this table.
        /// </summary>
        [IgnoreColumn]
        public IList<string> Natures { get; set; }

        [IgnoreColumn]
        public string NatureSummary
        {
            get { return Natures == null ? string.Empty : string.Join("; ", Natures); }
        }
    }
}
