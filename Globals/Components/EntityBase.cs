using System;

namespace tjc.Modules.Globals
{
    public class EntityBase
    {
        //Add common Properties here that will be used for all your entities
        public int CreatedById { get; set; }
        public int LastModifiedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
