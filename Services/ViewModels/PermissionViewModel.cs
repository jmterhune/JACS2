using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class PermissionViewModel
    {
        public PermissionViewModel(Permission permission)
        {
            id = permission.id;
            name = permission.name;
            guard_name = permission.guard_name;
        }

        public PermissionViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("guard_name")]
        public string guard_name { get; set; }
    }
}