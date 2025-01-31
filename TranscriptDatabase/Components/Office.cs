using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_office")]
    [PrimaryKey("OfficeID", AutoIncrement = true)]
    [Cacheable("Offices", CacheItemPriority.Default, 20)]
    internal class Office:EntityBase
    {
        public int OfficeID { get; set; }
        public string Description { get; set; }
        public int? DeliveryTypeID { get; set; }
        [IgnoreColumn]
        [EnumDataType(typeof(DeliveryTypes))]
        public DeliveryTypes DeliveryType
        {
            get
            {
                return (DeliveryTypes)this.DeliveryTypeID;
            }
            set
            {
                this.DeliveryTypeID = (int)value;
            }
        }
    }
    public enum DeliveryTypes
    {
        Interoffice = 0,
        UsPostage = 1
    }
}
