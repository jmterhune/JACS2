using System;

namespace tjc.Modules.TranscriptDatabase.Components
{
    public class EntityBase
    {
        public DateTime CreatedDate { get; set; }  // smalldatetime
        public int CreatedByUserID { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // smalldatetime
        public int LastModifiedByUserID { get; set; }  // int
    }
}