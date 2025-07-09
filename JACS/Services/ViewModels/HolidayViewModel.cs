using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class HolidayViewModel
    {
        public HolidayViewModel(Holiday holiday)
        {
            id = holiday.id;
            name = holiday.name;
            date = holiday.date;
        }

        public HolidayViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("date")]
        public DateTime date { get; set; }
    }
}