using Newtonsoft.Json;
using tjc.Modules.CourtRegistry.Components;
namespace tjc.Modules.CourtRegistry.Services
{
    [JsonObject(MemberSerialization.OptIn)]

    public class ApplicationViewModel
    {
        public ApplicationViewModel(ApplicationListItem application)
        {
            ApplicationID = application.ApplicationID;
            LastName = application.LastName;
            FirstName = application.FirstName;
            AttorneyID = application.AttorneyID;
            Status = application.Status;
            YearsOnRegistry = application.YearsOnRegistry;
            PeriodYear = string.Format("{0} - {1}", application.Year - 1, application.Year);
            Year = application.Year;
            DateCreated = application.DateCreated.ToShortDateString();
            if (application.DateReviewed.HasValue)
                DateReviewed = application.DateReviewed.Value.ToShortDateString();
            IsRenewal = application.IsRenewal;
            IsGuardian = application.IsGuardian;
            StatusName=application.StatusName;
        }
        public ApplicationViewModel() { }
        [JsonProperty("applicationid")]
        public int ApplicationID { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("attorneyid")]
        public int AttorneyID { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("statusname")]
        public string StatusName { get; set; }

        [JsonProperty("yearsonregistry")]
        public int YearsOnRegistry { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("periodyear")]
        public string PeriodYear { get; set; }

        [JsonProperty("datecreated")]
        public string DateCreated { get; set; }

        [JsonProperty("datereviewed")]
        public string DateReviewed { get; set; }

        [JsonProperty("isrenewal")]
        public bool IsRenewal { get; set; }

        [JsonProperty("isguardian")]
        public bool IsGuardian { get; set; }

    }
}
