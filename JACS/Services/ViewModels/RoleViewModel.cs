using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RoleViewModel
    {
        public RoleViewModel(Role role)
        {
            id = role.id;
            name = role.name;
            guard_name = role.guard_name;
        }

        public RoleViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("guard_name")]
        public string guard_name { get; set; }
    }
}