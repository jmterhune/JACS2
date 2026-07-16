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
        public int NhitProfileId { get; set; }  // int
        public string ProfileName { get; set; }  // varchar(100)
        public string BuildingLocation { get; set; }  // varchar(50)
        public string EmployeeType { get; set; }  // varchar(20)
        public bool EquipmentLaptop { get; set; }  // bit
        public bool EquipmentTwoInOne { get; set; }  // bit
        public bool EquipmentDesktop { get; set; }  // bit
        public bool EquipmentCellPhone { get; set; }  // bit
        public string AccessCardTo { get; set; }  // varchar(max)
        public string KeysNeeded { get; set; }  // varchar(max)
        public string ParkingAccess { get; set; }  // varchar(max)
        public string EmailDistributionGroups { get; set; }  // varchar(max)
        public string CalendarAccess { get; set; }  // varchar(max)
        public string ShareDriveAccess { get; set; }  // varchar(max)
        public string AdditionalPrinterAccess { get; set; }  // varchar(max)
        public bool ManagerBlog { get; set; }  // bit
        public bool AddToSupervisorDropdown { get; set; }  // bit
        public bool WorkCellphoneSetup { get; set; }  // bit
        public string Notes { get; set; }  // varchar(max)
        public DateTime CreatedDate { get; set; }  // datetime
        public int CreatedById { get; set; }  // int
        public DateTime LastModifiedDate { get; set; }  // datetime
        public int LastModifiedById { get; set; }  // int

        // Not persisted — populated by NhitProfileController on Get and read
        // by the API layer on Save so the round-trip carries the catalog
        // selections without a second endpoint call.
        [IgnoreColumn]
        public List<int> SelectedItemIds { get; set; }
    }
}
