using Newtonsoft.Json;
using System.Collections.Generic;

namespace tjc.Intranet.API.Services.ViewModels.FamilySelfHelp
{
    [JsonObject(MemberSerialization.OptIn)]

    public class ClientNameViewModel
    {
        public ClientNameViewModel(Components.FamilySelfHelp.ClientName clientName) {
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
