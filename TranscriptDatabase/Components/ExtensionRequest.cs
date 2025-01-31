using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_extension_request")]
    [PrimaryKey("ExtensionRequestID", AutoIncrement = true)]
    [Cacheable("ExtensionRequests", CacheItemPriority.Default, 20)]
    internal class ExtensionRequest:EntityBase
    {
        public int ExtensionID { get; set; }
        public int DesignationID { get; set; }
        public int EventTypeID { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? GrantedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public bool? Approved { get; set; }
    }
}
