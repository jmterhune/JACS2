using System;

namespace tjc.Modules.TranscriptDatabase.Components
{
    public class EntityBase
    {
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedByUserID { get; set; }
    }
}