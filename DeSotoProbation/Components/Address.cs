using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_common_address")]
    //setup the primary key for table
    [PrimaryKey("AddressID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Addresses", CacheItemPriority.Default, 20)]
    internal class Address
    {
        public long AddressID { get; set; }
        public string Street { get; set; }
        public string PO { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public bool IsPrimary { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public int LastModifiedBy { get; set; }
    }
}