using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_form")]
    [PrimaryKey("FormID", AutoIncrement = true)]
    [Cacheable("Forms", CacheItemPriority.Default, 20)]
    internal class Form :EntityBase
    {
        public int FormID { get; set; }
        public int? FileID { get; set; }
        public string FilePath { get; set; }
        public int? DocumentType { get; set; }
    }
}
