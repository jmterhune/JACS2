using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.jacs.Components
{
    
    public class ListItemOptionResult
    {
        [JsonProperty("data")]
        public List<KeyValuePair<long, string>> data { get; set; }
        [JsonProperty("error")]
        public string error { get; set; }
    }
    public static class Common
    {
        public static DateTime GetMondayOfCurrentWeek(DateTime currentDate)
        {
            int diff = ((int)currentDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return currentDate.AddDays(-diff).Date;
        }
    }
}