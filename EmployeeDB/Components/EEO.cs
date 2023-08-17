using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_eeo")]
    //setup the primary key for table
    [PrimaryKey("EeoId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("EEOs", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class EEO : EmployeeBase
    {//Employment Equal Opportunity
        public long EeoId { get; set; }

        public int? JobGroupId { get; set; }

        public decimal? PopulationMale { get; set; }

        public decimal? PopulationFemale { get; set; }

        public decimal? PopulationWhite { get; set; }

        public decimal? PopulationIndian { get; set; }

        public decimal? PopulationBlack { get; set; }

        public decimal? PopulationAsian { get; set; }

        public decimal? PopulationHispanic { get; set; }

        public decimal? PopulationOther { get; set; }

        public decimal? HireMale { get; set; }

        public decimal? HireFemale { get; set; }

        public decimal? HireWhite { get; set; }

        public decimal? HireBlack { get; set; }

        public decimal? HireAsian { get; set; }

        public decimal? HireIndian { get; set; }

        public decimal? HireHispanic { get; set; }

        public decimal? HireOther { get; set; }

        public decimal? PromoMale { get; set; }

        public decimal? PromoFemale { get; set; }

        public decimal? PromoWhite { get; set; }

        public decimal? PromoBlack { get; set; }

        public decimal? PromoAsian { get; set; }

        public decimal? PromoIndian { get; set; }

        public decimal? PromoHispanic { get; set; }

        public decimal? PromoOther { get; set; }

        public decimal? TransferMale { get; set; }

        public decimal? TransferFemale { get; set; }

        public decimal? TransferWhite { get; set; }

        public decimal? TransferBlack { get; set; }

        public decimal? TransferAsian { get; set; }

        public decimal? TransferIndian { get; set; }

        public decimal? TransferHispanic { get; set; }

        public decimal? TransferOther { get; set; }

        public decimal? TermMale { get; set; }

        public decimal? TermFemale { get; set; }

        public decimal? TermWhite { get; set; }

        public decimal? TermBlack { get; set; }

        public decimal? TermIndian { get; set; }

        public decimal? TermAsian { get; set; }

        public decimal? TermHispanic { get; set; }

        public decimal? TermOther { get; set; }

        public int? Year { get; set; }
    }
    [TableName("tjc_employee_eeo_list")]
    //setup the primary key for table
    [PrimaryKey("EeoId", AutoIncrement = false)]
    //configure caching using PetaPoco
    internal class EeoListItem:EmployeeBase
    {
        public string JobGroupName { get; set; }
        public long EeoId { get; set; }

        public int? JobGroupId { get; set; }

        public decimal? PopulationMale { get; set; }

        public decimal? PopulationFemale { get; set; }

        public decimal? PopulationWhite { get; set; }

        public decimal? PopulationIndian { get; set; }

        public decimal? PopulationBlack { get; set; }

        public decimal? PopulationAsian { get; set; }

        public decimal? PopulationHispanic { get; set; }

        public decimal? PopulationOther { get; set; }

        public decimal? HireMale { get; set; }

        public decimal? HireFemale { get; set; }

        public decimal? HireWhite { get; set; }

        public decimal? HireBlack { get; set; }

        public decimal? HireAsian { get; set; }

        public decimal? HireIndian { get; set; }

        public decimal? HireHispanic { get; set; }

        public decimal? HireOther { get; set; }

        public decimal? PromoMale { get; set; }

        public decimal? PromoFemale { get; set; }

        public decimal? PromoWhite { get; set; }

        public decimal? PromoBlack { get; set; }

        public decimal? PromoAsian { get; set; }

        public decimal? PromoIndian { get; set; }

        public decimal? PromoHispanic { get; set; }

        public decimal? PromoOther { get; set; }

        public decimal? TransferMale { get; set; }

        public decimal? TransferFemale { get; set; }

        public decimal? TransferWhite { get; set; }

        public decimal? TransferBlack { get; set; }

        public decimal? TransferAsian { get; set; }

        public decimal? TransferIndian { get; set; }

        public decimal? TransferHispanic { get; set; }

        public decimal? TransferOther { get; set; }

        public decimal? TermMale { get; set; }

        public decimal? TermFemale { get; set; }

        public decimal? TermWhite { get; set; }

        public decimal? TermBlack { get; set; }

        public decimal? TermIndian { get; set; }

        public decimal? TermAsian { get; set; }

        public decimal? TermHispanic { get; set; }

        public decimal? TermOther { get; set; }

        public int? Year { get; set; }
    }
}
