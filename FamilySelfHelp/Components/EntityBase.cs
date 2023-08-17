using System;

namespace tjc.Modules.FamilySelfHelp.Components
{
    public class EntityBase
    {
        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int LastModifiedById { get; set; }
    }
}