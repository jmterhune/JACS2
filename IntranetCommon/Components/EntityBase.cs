using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tjc.Modules.IntranetCommon.Components
{
    public class EntityBase
    {
        //Add common Properties here that will be used for all your entities
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
