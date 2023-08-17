using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.MediationStatistics.Components
{
    public class EntityBase
    {
            public DateTime CreatedDate { get; set; }

            public int CreatedById { get; set; }

            public DateTime LastModifiedDate { get; set; }

            public int LastModifiedById { get; set; }
    }
}