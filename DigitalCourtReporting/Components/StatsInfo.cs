using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.DigitalCourtReporting.Components
{
    public class StatsInfo
    {
       
        public string Heading { get; set; }
        public int TotalNumber { get; set; }
        public int MinBurned { get; set; }

    }
    public class StatRecord
    {
        public int CDCount { get; set; }
        public int TotalMinutes { get; set; }
    }
   
}