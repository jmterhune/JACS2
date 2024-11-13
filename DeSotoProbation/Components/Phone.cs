using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_common_phone")]
    //setup the primary key for table
    [PrimaryKey("PhoneID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Phones", CacheItemPriority.Default, 20)]
    internal class Phone
    {
        public long PhoneID { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public string Extension { get; set; }
        public string Type { get; set; }
        public bool IsPrimary { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public int LastModifiedBy { get; set; }
    }
}