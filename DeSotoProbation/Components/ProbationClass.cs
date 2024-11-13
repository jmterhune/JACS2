using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_desoto_probation_classes")]
    //setup the primary key for table
    [PrimaryKey("ClassID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Classes", CacheItemPriority.Default, 20)]
    internal class ProbationClass
    {
        public int ClassID { get; set; }
        public string Name { get; set; }

    }
    [TableName("tjc_desoto_probation_class_xref")]
    //setup the primary key for table
    internal class ProbationClassXref
    {
        public int ClassID { get; set; }
        public int RecordID { get; set; }
    }
}