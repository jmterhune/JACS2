using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.ProSeLog.Components;

namespace tjc.Modules.ProSeLog.Components.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]

    public class CaseNumberViewModel
    {
        public CaseNumberViewModel() { }
        public CaseNumberViewModel(CaseNumber casenumber)
        {
            Text = casenumber.Text;
        }


        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
