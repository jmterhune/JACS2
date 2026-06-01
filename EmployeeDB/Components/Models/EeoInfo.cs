using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    [TableName("tjc_employee_eeo")]
    [PrimaryKey("EeoId", AutoIncrement = true)]
    public class EeoInfo
    {
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
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }

        /// <summary>Resolved JobGroup.Description, populated by the API layer
        /// on read so the list can render the category name without a second
        /// lookup. Excluded from PetaPoco INSERT / UPDATE / SELECT.</summary>
        [IgnoreColumn]
        public string JobGroupName { get; set; }
    }
}
