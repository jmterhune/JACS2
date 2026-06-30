using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_eeo")]
    [PrimaryKey("EeoId", AutoIncrement = true)]
    public class EeoInfo
    {
        public long EeoId { get; set; }  // bigint
        public int? JobGroupId { get; set; }  // int
        public decimal? PopulationMale { get; set; }  // decimal(18,4)
        public decimal? PopulationFemale { get; set; }  // decimal(18,4)
        public decimal? PopulationWhite { get; set; }  // decimal(18,4)
        public decimal? PopulationIndian { get; set; }  // decimal(18,4)
        public decimal? PopulationBlack { get; set; }  // decimal(18,4)
        public decimal? PopulationAsian { get; set; }  // decimal(18,4)
        public decimal? PopulationHispanic { get; set; }  // decimal(18,4)
        public decimal? PopulationOther { get; set; }  // decimal(18,4)
        public decimal? HireMale { get; set; }  // decimal(18,4)
        public decimal? HireFemale { get; set; }  // decimal(18,4)
        public decimal? HireWhite { get; set; }  // decimal(18,4)
        public decimal? HireBlack { get; set; }  // decimal(18,4)
        public decimal? HireAsian { get; set; }  // decimal(18,4)
        public decimal? HireIndian { get; set; }  // decimal(18,4)
        public decimal? HireHispanic { get; set; }  // decimal(18,4)
        public decimal? HireOther { get; set; }  // decimal(18,4)
        public decimal? PromoMale { get; set; }  // decimal(18,4)
        public decimal? PromoFemale { get; set; }  // decimal(18,4)
        public decimal? PromoWhite { get; set; }  // decimal(18,4)
        public decimal? PromoBlack { get; set; }  // decimal(18,4)
        public decimal? PromoAsian { get; set; }  // decimal(18,4)
        public decimal? PromoIndian { get; set; }  // decimal(18,4)
        public decimal? PromoHispanic { get; set; }  // decimal(18,4)
        public decimal? PromoOther { get; set; }  // decimal(18,4)
        public decimal? TransferMale { get; set; }  // decimal(18,4)
        public decimal? TransferFemale { get; set; }  // decimal(18,4)
        public decimal? TransferWhite { get; set; }  // decimal(18,4)
        public decimal? TransferBlack { get; set; }  // decimal(18,4)
        public decimal? TransferAsian { get; set; }  // decimal(18,4)
        public decimal? TransferIndian { get; set; }  // decimal(18,4)
        public decimal? TransferHispanic { get; set; }  // decimal(18,4)
        public decimal? TransferOther { get; set; }  // decimal(18,4)
        public decimal? TermMale { get; set; }  // decimal(18,4)
        public decimal? TermFemale { get; set; }  // decimal(18,4)
        public decimal? TermWhite { get; set; }  // decimal(18,4)
        public decimal? TermBlack { get; set; }  // decimal(18,4)
        public decimal? TermIndian { get; set; }  // decimal(18,4)
        public decimal? TermAsian { get; set; }  // decimal(18,4)
        public decimal? TermHispanic { get; set; }  // decimal(18,4)
        public decimal? TermOther { get; set; }  // decimal(18,4)
        public int? Year { get; set; }  // int
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int

        /// <summary>Resolved JobGroup.Description, populated by the API layer
        /// on read so the list can render the category name without a second
        /// lookup. Excluded from PetaPoco INSERT / UPDATE / SELECT.</summary>
        [IgnoreColumn]
        public string JobGroupName { get; set; }
    }
}
