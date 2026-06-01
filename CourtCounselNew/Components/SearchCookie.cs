using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.CourtCounsel.Components
{
    public class SearchCookie
    {
        public SearchType searchType { get; set; }
        public int AttorneyId { get; set; }
        public bool Active { get; set; }
        public bool Pending { get; set; }
        public bool Closed { get; set; }
        public string SearchTerm { get; set; }
    }
}