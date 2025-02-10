using System;

namespace tjc.Modules.TranscriptDatabase.Components
{
    public class EntityBase
    {
        public DateTime CreatedDate { get; set; }
        public int CreatedByUser { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedByUser { get; set; }
    }
}