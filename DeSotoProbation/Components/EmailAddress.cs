using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_common_email")]
    //setup the primary key for table
    [PrimaryKey("EmailID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("EmailAddresses", CacheItemPriority.Default, 20)]
    internal class EmailAddress
    {
        public long EmailAddressID { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public bool IsPrimary { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public int LastModifiedBy { get; set; }
    }
}