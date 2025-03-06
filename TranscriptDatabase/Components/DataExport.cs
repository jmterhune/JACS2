using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.TranscriptDatabase.Components
{
    public class DocumentDataExport
    {
        public string TranscriptFiled { get; set; }

        public string MailType { get; set; }

        public string ExtensionReason { get; set; }

        public string ExtensionDays { get; set; }

        public string EstimatedPages { get; set; }

        public string DesignatingAttorney { get; set; }

        public string Defendant { get; set; }

        public string DaysDesignated { get; set; }

        public string DateReceived { get; set; }

        public string CreatedDate { get; set; }

        public string CourtReporter { get; set; }

        public string County { get; set; }

        public string CircuitCounty { get; set; }

        public string CaseNumber { get; set; }

        public string DCACaseNumber { get; set; }
    }
}