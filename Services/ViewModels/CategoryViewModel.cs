using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CategoryViewModel
    {
        public CategoryViewModel(Category category)
        {
            id = category.id;
            description = category.description;
        }

        public CategoryViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
    }
}