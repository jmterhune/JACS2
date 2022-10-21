using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.JudicialReferral.Components
{
    [TableName("tjc_cc_case_type")]
    //setup the primary key for table
    [PrimaryKey("CaseTypeID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("CaseTypes", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class CaseType
    {
        public int CaseTypeID { get; set; }
        public string CaseTypeName { get; set; }
    }
}