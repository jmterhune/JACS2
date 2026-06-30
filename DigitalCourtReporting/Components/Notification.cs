using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    [TableName("tjc_dcr_notification")]
    [PrimaryKey("DeliveryID", AutoIncrement = true)]
    [Cacheable("Notifications", CacheItemPriority.Default, 20)]
    internal class Notification : EntityBase
    {
        public int DeliveryID { get; set; }            // int
        public int ProceedingID { get; set; }          // int
        public string Description { get; set; }         // nvarchar(14)
        public string DateCalled { get; set; }          // nvarchar(12)
        public string Responsible { get; set; }         // nvarchar(50)
        public string PersonCalled { get; set; }        // nvarchar(50)
        public string ReceivedBy { get; set; }          // nvarchar(50)
        public string PickupDate { get; set; }          // nvarchar(12)
        public string Notes { get; set; }               // nvarchar(max)
    }
}
