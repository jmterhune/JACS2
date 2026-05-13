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
        public int NhitRequestId { get; set; }
        public int? EmployeeId { get; set; }

        // Employee-unique fields ----------------------------------------
        public string EmployeeName { get; set; }
        public string AKA { get; set; }
        public string PositionTitle { get; set; }
        public string SupervisorName { get; set; }
        public string DepartmentUnitGroup { get; set; }
        public string OfficeSuiteNumber { get; set; }
        public string DeskPhoneNumber { get; set; }
        public DateTime? TodaysDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? TempInternEndDate { get; set; }

        // Profile-derived fields ----------------------------------------
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

        // Submission metadata -------------------------------------------
        public DateTime SubmittedDate { get; set; }
        public int SubmittedById { get; set; }
        public string EmailSentTo { get; set; }
        public DateTime? EmailSentDate { get; set; }
        public bool? EmailSuccess { get; set; }
        public string EmailErrorMessage { get; set; }

        // Not persisted — populated client-side on POST, fanned out into
        // tjc_nhit_request_item rows by NhitRequestController.Create.
        [IgnoreColumn]
        public List<int> SelectedItemIds { get; set; }
    }
}
