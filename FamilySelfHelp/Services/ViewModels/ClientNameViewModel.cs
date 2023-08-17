using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.FamilySelfHelp.Components;

namespace tjc.Modules.FamilySelfHelp.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]

    public class ClientNameViewModel
    {
        public ClientNameViewModel(ClientName clientName) {
            Text = clientName.Text;
            Value = clientName.Value;
        }
        public ClientNameViewModel() { }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

    }
}
