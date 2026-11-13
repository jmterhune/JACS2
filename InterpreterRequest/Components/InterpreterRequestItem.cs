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

namespace tjc.Modules.InterpreterRequest.Components
{
    /// <summary>
    /// One interpreter request submitted on the public site. The rows live in the
    /// public (jud12) database and are reached from here through the "Jud12"
    /// connection, so the record is deliberately not scoped to ModuleId: the
    /// ModuleId on it belongs to the public site's form module, not to anything
    /// on this intranet site.
    /// </summary>
    [TableName("tjc_modules_interpreter_requests")]
    [PrimaryKey("InterpreterRequestId", AutoIncrement = true)]
    public class InterpreterRequestItem
    {
        public int InterpreterRequestId { get; set; }

        public int ModuleId { get; set; }

        public int PortalId { get; set; }

        public DateTime CreatedOnDate { get; set; }

        /// <summary>True when the visitor filled in the Spanish version of the form.</summary>
        public bool IsSpanishForm { get; set; }

        // ---- Interpreter language ----

        /// <summary>The language picked from the list, or the typed-in language when "Other" was picked.</summary>
        public string InterpreterLanguage { get; set; }

        /// <summary>Whatever was typed in the "other language" box, kept verbatim.</summary>
        public string OtherLanguage { get; set; }

        // ---- Case information ----

        public string CaseNumber { get; set; }

        public string PersonNeeding { get; set; }

        public string OtherPersonNeeding { get; set; }

        public string PersonLastName { get; set; }

        public string PersonFirstName { get; set; }

        public string EventType { get; set; }

        public string OtherEventType { get; set; }

        // ---- When ----

        /// <summary>Kept as typed: the form takes free text through a date picker.</summary>
        public string DateNeeded { get; set; }

        public string TimeNeeded { get; set; }

        public string DurationNeeded { get; set; }

        // ---- Where ----

        public string County { get; set; }

        public string Courtroom { get; set; }

        /// <summary>"In-Person" or "Zoom".</summary>
        public string AppearanceMethod { get; set; }

        public string JudgeName { get; set; }

        // ---- Who asked ----

        public string RequestorLastName { get; set; }

        public string RequestorFirstName { get; set; }

        public string RequestorPhone { get; set; }

        public string RequestorEmail { get; set; }

        public bool IsAttorney { get; set; }

        // ---- Handling, set here ----

        public string Status { get; set; }

        public string StatusNotes { get; set; }

        public int? StatusChangedByUserId { get; set; }

        public DateTime? StatusChangedOnDate { get; set; }

        [IgnoreColumn]
        public string RequestorName
        {
            get { return string.Format("{0}, {1}", RequestorLastName, RequestorFirstName).Trim(' ', ','); }
        }

        [IgnoreColumn]
        public string PersonName
        {
            get { return string.Format("{0}, {1}", PersonLastName, PersonFirstName).Trim(' ', ','); }
        }

        [IgnoreColumn]
        public string WhenNeeded
        {
            get { return string.Format("{0} {1}", DateNeeded, TimeNeeded).Trim(); }
        }
    }
}
