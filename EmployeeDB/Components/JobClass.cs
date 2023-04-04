using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_class")]
    //setup the primary key for table
    [PrimaryKey("ClassId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("JobClasses", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class JobClass : EmployeeBase
    {
        public int ClassId { get; set; }

        public string ClassCode { get; set; }

        public string ClassName { get; set; }

        public string PayGrade { get; set; }

        public string FLSA { get; set; }

        public string EEO { get; set; }

        public decimal? MMax { get; set; }

        public decimal? MMin { get; set; }

        public decimal? AMax { get; set; }

        public decimal? AMin { get; set; }

    }
}