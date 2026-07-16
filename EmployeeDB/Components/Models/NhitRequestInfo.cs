using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    /// <summary>
    /// Submitted New Hire IT Worksheet — captures EVERY field on the form
    /// (employee-unique fields plus the catalog checkbox state) at the moment
    /// it was emailed to the helpdesk. Acts as an audit log: even if a
    /// profile is later deleted or items renamed, the original submission
    /// can be reconstructed verbatim.
    /// </summary>
    [TableName("tjc_nhit_request")]
    [PrimaryKey("NhitRequestId", AutoIncrement = true)]
    public class NhitRequestInfo
    {
        public int NhitRequestId { get; set; }  // int
        public int? EmployeeId { get; set; }  // int

        // Employee-unique fields ----------------------------------------
        public string EmployeeName { get; set; }  // varchar(200)
        public string AKA { get; set; }  // varchar(100)
        public string PositionTitle { get; set; }  // varchar(150)
        public string SupervisorName { get; set; }  // varchar(200)
        public string DepartmentUnitGroup { get; set; }  // varchar(150)
        public string OfficeSuiteNumber { get; set; }  // varchar(50)
        public string DeskPhoneNumber { get; set; }  // varchar(50)
        public DateTime? TodaysDate { get; set; }  // date
        public DateTime? EffectiveDate { get; set; }  // date
        public DateTime? TempInternEndDate { get; set; }  // date

        // Profile-derived fields ----------------------------------------
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

        // Submission metadata -------------------------------------------
        public DateTime SubmittedDate { get; set; }  // datetime
        public int SubmittedById { get; set; }  // int
        public string EmailSentTo { get; set; }  // varchar(500)
        public DateTime? EmailSentDate { get; set; }  // datetime
        public bool? EmailSuccess { get; set; }  // bit
        public string EmailErrorMessage { get; set; }  // varchar(max)

        // Not persisted — populated client-side on POST, fanned out into
        // tjc_nhit_request_item rows by NhitRequestController.Create.
        [IgnoreColumn]
        public List<int> SelectedItemIds { get; set; }
    }
}
