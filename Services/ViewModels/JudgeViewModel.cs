using DocumentFormat.OpenXml.Wordprocessing;
using DotNetNuke.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class JudgeViewModel
    {
        public JudgeViewModel(Judge judge)
        {
            id = judge.id;
            user_id = judge.user_id;
            name = judge.name;
            phone = judge.phone;
            court_name = judge.court_id.HasValue ? GetJudgeCourtName(judge.court_id.Value) : "";
            court_id = judge.court_id ?? 0;
            title = judge.title;
        }

        public JudgeViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("user_id")]
        public long user_id { get; set; }

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
    [JsonObject(MemberSerialization.OptIn)]
    internal class JudgeClerkXrefViewModel
    {
        public JudgeClerkXrefViewModel(JudgeClerkXref judge)
        {
            judge_id = judge.judge_id;
            county_id = judge.county_id;
            clerk_judge_id = judge.clerk_judge_id;
            clerk_judge_name = judge.clerk_judge_name;
        }
        public JudgeClerkXrefViewModel(JudgeClerkXrefListItem judge)
        {
            judge_id = judge.judge_id;
            county_id = judge.county_id;
            clerk_judge_id = judge.clerk_judge_id;
            clerk_judge_name = judge.clerk_judge_name;
            county_name = judge.county_name ?? string.Empty;
            judge_name = judge.judge_name ?? string.Empty;
        }
        public JudgeClerkXrefViewModel() { }

        [JsonProperty("judge_id")]
        public long judge_id { get; set; }
        [JsonProperty("county_id")]
        public long county_id { get; set; }
        [JsonProperty("clerk_judge_id")]
        public long clerk_judge_id { get; set; }
        [JsonProperty("clerk_judge_name")]
        public string clerk_judge_name { get; set; }
        [JsonProperty("county_name")]
        public string county_name { get; set; } 
        [JsonProperty("judge_name")]
        public string judge_name { get; set; } 
    }
}