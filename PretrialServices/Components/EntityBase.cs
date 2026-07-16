using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.PretrialServices.Components
{
    public class EntityBase
    {
        public DateTime CreatedDate { get; set; } // smalldatetime

        public int CreatedById { get; set; } // int

        public DateTime LastModifiedDate { get; set; } // smalldatetime

        public int LastModifiedById { get; set; } // int
    }
}