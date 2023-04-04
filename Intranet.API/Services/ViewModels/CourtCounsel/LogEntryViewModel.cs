using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Intranet.API.Components.CourtCounsel;
namespace tjc.Intranet.API.Services.ViewModels.CourtCounsel
{
    [JsonObject(MemberSerialization.OptIn)]

    public class LogEntryViewModel
    {
        public LogEntryViewModel(LogEntry logEntry) {
            LogId = logEntry.LogId;
            CaseNumber = logEntry.CaseNumber;
            Description = logEntry.Description;
            IsCase = logEntry.IsCase;
            CountyId = logEntry.CountyId;
        }
        public LogEntryViewModel() { }

        [JsonProperty("logId")]
        public long LogId { get; set; }

        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("isCase")]
        public bool IsCase { get; set; }
        [JsonProperty("countyId")]
        public int CountyId { get; set; }

    }
}
