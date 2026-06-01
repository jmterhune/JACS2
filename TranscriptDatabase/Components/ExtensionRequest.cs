using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_extension_request")]
    [PrimaryKey("ExtensionID", AutoIncrement = true)]
    [Cacheable("ExtensionRequests", CacheItemPriority.Default, 20)]
    public class ExtensionRequest : EntityBase
    {
        public int ExtensionID { get; set; }
        public int DesignationID { get; set; }
        public int EventTypeID { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? GrantedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public bool Approved { get; set; }
        [IgnoreColumn]
        public EventTypes EventType
        {
            get
            {
                return (EventTypes)EventTypeID;
            }
        }
        [IgnoreColumn]
        public string EventTypeName
        {
            get
            {
                return Enumerations.GetEnumDescription((EventTypes)EventTypeID);
            }
        }
        [IgnoreColumn]
        public string RequestedDateFormatted
        {
            get
            {
                if (RequestedDate.HasValue)
                    return RequestedDate.Value.ToShortDateString(); 
                return "";
            }
        }
        [IgnoreColumn]
        public string GrantedDateFormatted
        {
            get
            {
                if (GrantedDate.HasValue)
                    return GrantedDate.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string SubmittedDateFormatted
        {
            get
            {
                if (SubmittedDate.HasValue)
                    return SubmittedDate.Value.ToShortDateString();
                return "";
            }
        }
    }
}
