using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_desoto_probation_programs")]
    //setup the primary key for table
    [PrimaryKey("ProgramID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("ProgramNames", CacheItemPriority.Default, 20)]
    internal class Program
    {
        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
    }
}