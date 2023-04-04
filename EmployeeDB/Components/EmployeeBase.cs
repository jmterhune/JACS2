using System;

namespace tjc.Modules.EmployeeDB.Components
{
    public class EmployeeBase
    {
        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int LastModifiedById { get; set; }
    }
}