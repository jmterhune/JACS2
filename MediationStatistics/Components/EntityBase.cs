using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.MediationStatistics.Components
{
    public class EntityBase
    {
            public DateTime CreatedDate { get; set; }  // datetime

            public int CreatedById { get; set; }  // int

            public DateTime LastModifiedDate { get; set; }  // datetime

            public int LastModifiedById { get; set; }  // int
    }
}