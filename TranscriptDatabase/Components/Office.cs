using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_office")]
    [PrimaryKey("OfficeID", AutoIncrement = true)]
    [Cacheable("tjc_rec_Offices", CacheItemPriority.Default, 20)]
    internal class Office : EntityBase
    {
        public int OfficeID { get; set; }  // int
        public string Description { get; set; }  // nvarchar(100)
        public int? DeliveryTypeID { get; set; }  // int
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
        [IgnoreColumn]
        public string DeliveryTypeeName
        {
            get
            {
                if (DeliveryTypeID.HasValue)
                    return Enumerations.GetEnumDescription(DeliveryType);
                return "";
            }
        }
    }
}
