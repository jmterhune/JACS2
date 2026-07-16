using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    // Global groups (Departments) table. Schema: GroupType and IsSwnGroup are
    // NOT NULL with DEFAULT(0) on the DB. The model used to declare them as
    // int? / bool?, which made PetaPoco serialize them as NULL on INSERT — that
    // violated the constraint and caused Add/Edit Department to fail. They are
    // non-nullable here so a default-constructed GroupInfo (e.g. JSON binding
    // from the admin tab that doesn't send GroupType) produces a valid INSERT.
    [TableName("tjc_gl_group")]
    [PrimaryKey("GroupID", AutoIncrement = true)]
    [Cacheable("tjc_gl_group", CacheItemPriority.Default, 20)]
    public class GroupInfo
    {
        public int GroupID { get; set; }  // int
        public string GroupName { get; set; }  // nvarchar(50)
        public int GroupType { get; set; }  // int
        public bool IsSwnGroup { get; set; }  // bit
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedByID { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedByID { get; set; }  // int
    }
}
