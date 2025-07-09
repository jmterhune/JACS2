using Newtonsoft.Json;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class JudgeViewModel
    {
        public JudgeViewModel(Judge judge)
        {
            id = judge.id;
            name = judge.name;
            phone = judge.phone;
            court_name = GetJudgeCourtName(judge.court_id.Value);
            court_id = judge.court_id ?? 0;
            title = judge.title;
        }

        public JudgeViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("phone")]
        public string phone { get; set; }

        [JsonProperty("court_id")]
        public long court_id { get; set; }

        [JsonProperty("court_name")]
        public string court_name { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }
        public string GetJudgeCourtName(long id)
        {
            var ctl = new CourtController();
            return ctl.GetCourt(id).description ?? string.Empty;
        }
    }
}