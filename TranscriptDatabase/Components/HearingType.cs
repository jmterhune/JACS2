using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_hearing_type")]
    [PrimaryKey("HearingTypeID", AutoIncrement = true)]
    [Cacheable("HearingTypes", CacheItemPriority.Default, 20)]
    internal class HearingType:EntityBase
    {
        public int HearingTypeID { get; set; }  // int
        public string HearingTypeName { get; set; }  // nvarchar(50)
    }
}