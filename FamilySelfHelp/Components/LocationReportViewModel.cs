using System.Collections.Generic;

namespace tjc.Modules.FamilySelfHelp.Components
{
    public class LocationReportViewModel
    {
        public string Location { get; set; }
        public IEnumerable<CountItem> ClientTypes { get; set; }
        public IEnumerable<CountItem> ContactMethods { get; set; }
        public IEnumerable<CountItem> CaseTypes { get; set; }
        public IEnumerable<CountItem> Services { get; set; }
        public IEnumerable<CountItem> Divisions { get; set; }

        public int InterpreterRequested { get; set; }
        public int NewCases { get; set; }
        public decimal TotalTime { get; set; }
        public decimal AverageTime { get; set; }
        public int UniqueCustomers { get; set; }
    }

    public class CountItem
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class LocationStat
    {
        public string Location { get; set; }

        public int TotalRecords { get; set; }
        public int InterpreterRequested { get; set; }
        public int NewCases { get; set; }
        public decimal TotalTime { get; set; }
        public decimal AverageTime { get; set; }
        public int UniqueClients { get; set; }

        public string ClientTypeSummary { get; set; }     // e.g. "Walk-in:18, Phone:27"
        public string ContactMethodSummary { get; set; }
        public string DivisionSummary { get; set; }
    }
}