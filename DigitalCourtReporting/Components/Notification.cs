using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    [TableName("tjc_dcr_notification")]
    [PrimaryKey("NotificationID", AutoIncrement = true)]
    [Cacheable("Notifications", CacheItemPriority.Default, 20)]
    internal class Notification : EntityBase
    {
        public int DeliveryID { get; set; }
        public int ProceedingID { get; set; }
        public string Description { get; set; }
        public string DateCalled { get; set; }
        public string Responsible { get; set; }
        public string PersonCalled { get; set; }
        public string ReceivedBy { get; set; }
        public string PickupDate { get; set; }
        public string Notes { get; set; }
    }
}
