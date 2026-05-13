using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    /// <summary>
    /// A saved set of New Hire IT Worksheet defaults — everything except the
    /// employee-unique fields (name, AKA, position, supervisor, office #,
    /// desk phone, dates). Loading a profile pre-populates those non-unique
    /// fields and toggles the corresponding catalog checkboxes.
    ///
    /// SelectedItemIds is an ignored-column property populated by the
    /// controller so the JS layer gets the catalog checkbox state along
    /// with the profile header in a single API call.
    /// </summary>
    [TableName("tjc_nhit_profile")]
    [PrimaryKey("NhitProfileId", AutoIncrement = true)]
    public class NhitProfileInfo
    {
        public int NhitProfileId { get; set; }
        public string ProfileName { get; set; }
        public string BuildingLocation { get; set; }
        public string EmployeeType { get; set; }
        public bool EquipmentLaptop { get; set; }
        public bool EquipmentTwoInOne { get; set; }
        public bool EquipmentDesktop { get; set; }
        public bool EquipmentCellPhone { get; set; }
        public string AccessCardTo { get; set; }
        public string KeysNeeded { get; set; }
        public string ParkingAccess { get; set; }
        public string EmailDistributionGroups { get; set; }
        public string CalendarAccess { get; set; }
        public string ShareDriveAccess { get; set; }
        public string AdditionalPrinterAccess { get; set; }
        public bool ManagerBlog { get; set; }
        public bool AddToSupervisorDropdown { get; set; }
        public bool WorkCellphoneSetup { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedById { get; set; }

        // Not persisted — populated by NhitProfileController on Get and read
        // by the API layer on Save so the round-trip carries the catalog
        // selections without a second endpoint call.
        [IgnoreColumn]
        public List<int> SelectedItemIds { get; set; }
    }
}
