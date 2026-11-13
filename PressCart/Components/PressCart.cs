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

namespace tjc.Modules.PressCart.Components
{
    /// <summary>
    /// One presentation cart request submitted on the public site. The rows live in
    /// the public (jud12) database and are reached from here through the "Jud12"
    /// connection, so unlike the public module's copy this one is deliberately not
    /// scoped to ModuleId: the ModuleId on the row belongs to the public site's form
    /// module, not to anything on this intranet site.
    /// </summary>
    [TableName("tjc_PressCart")]
    [PrimaryKey("PressCartId", AutoIncrement = true)]
    public class PressCart
    {
        public int PressCartId { get; set; }

        public int ModuleId { get; set; }

        /// <summary>Nullable in tjc_pressCart, though every row written so far has one.</summary>
        public DateTime? CreatedOnDate { get; set; }

        // ---- Requestor ----

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        // ---- Contact ----

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        // ---- Hearing ----

        /// <summary>Kept as typed: the form takes free text through a date picker.</summary>
        public string CourtDate { get; set; }

        public string CourtTime { get; set; }

        public string County { get; set; }

        public string Courthouse { get; set; }

        public string Courtroom { get; set; }

        /// <summary>The division and judge the request was filed under.</summary>
        public string Judge { get; set; }

        // ---- Equipment ----
        //
        // Every one of these is nullable in tjc_pressCart, and thousands of the older
        // rows really are null (Zoom and TrialLength especially: both were added to the
        // table long after the form started collecting requests). Declaring them
        // non-nullable here would throw while reading those rows, so they match the
        // column instead.

        public bool? TelevisionCombo { get; set; }

        public bool? Laptop { get; set; }

        public bool? Projector { get; set; }

        public bool? DVD { get; set; }

        public bool? VCR { get; set; }

        public bool? Cassette { get; set; }

        public bool? DocumentCamera { get; set; }

        public bool? XRay { get; set; }

        public bool? Zoom { get; set; }

        public string Comments { get; set; }

        public bool? TrainingRequired { get; set; }

        /// <summary>"Presentation Cart" or "Zoom Cart".</summary>
        public string CartType { get; set; }

        /// <summary>Days booked, asked for only on some Sarasota divisions; null when not asked.</summary>
        public int? TrialLength { get; set; }

        // ---- Handling, set here ----

        public string Status { get; set; }

        public string StatusNotes { get; set; }

        public int? StatusChangedByUserId { get; set; }

        public DateTime? StatusChangedOnDate { get; set; }

        [IgnoreColumn]
        public string RequestorName
        {
            get { return string.Format("{0}, {1}", LastName, FirstName).Trim(' ', ','); }
        }

        [IgnoreColumn]
        public string WhenNeeded
        {
            get { return string.Format("{0} {1}", CourtDate, CourtTime).Trim(); }
        }
    }
}
