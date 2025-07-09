/*
' Copyright (c) 2019 jud12
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

namespace tjc.Modules.ThreatReport.Components
{
    [TableName("tjc_threat_incident")]
    //setup the primary key for table
    [PrimaryKey("IncidentID", AutoIncrement = true)]
    class Incident
    {
        public int IncidentID { get; set; }

        public string Location { get; set; }

        public string ReportedBy { get; set; }

        public string NatureOfIncident { get; set; }

        public string Description { get; set; }

        public string PersonTargeted { get; set; }

        public string ActionTaken { get; set; }

        public string ReporterPhone { get; set; }

        public string ReporterExt { get; set; }

        public string ReporterEmail { get; set; }

        public string PersonReportingToLEO { get; set; }

        public string CaseNumber { get; set; }

        public string LEOAgency { get; set; }

        public bool IsCourtEmployee { get; set; }

        public bool WasTargetNotified { get; set; }

        public int CreatedByUserID { get; set; }

        public DateTime DateOfIncident { get; set; }

        public DateTime DateReported { get; set; }

        public DateTime DateReportedLEO { get; set; }

        [IgnoreColumn]
        public IEnumerable<Person> Persons { get; set; }

        [IgnoreColumn]
        public IEnumerable<Attachment> Attachments { get; set; }
    }
}
