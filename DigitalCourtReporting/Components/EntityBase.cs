using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tjc.Modules.DigitalCourtReporting.Components
{
    public class EntityBase
    {
        public int? CreatedByID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByID { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
}