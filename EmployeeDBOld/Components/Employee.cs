using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using tjc.Modules.Globals;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee")]
    //setup the primary key for table
    [PrimaryKey("EmployeeId", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class Employee : EmployeeBase
    {
        public int EmployeeId { get; set; }
        public int? SupervisorId { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobGroupID { get; set; }
        public int? ClassId { get; set; }
        public int? OfficeLocationId { get; set; }
        public int? CountyId { get; set; }
        public string LocationName { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string BadgeNumber { get; set; }
        public string Position { get; set; }
        public string EmploymentType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int PhotoFileId { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? ServiceDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }
        public string JobTitle { get; set; }
        public decimal? Salary { get; set; }
        public decimal? AnnualLeaveBalance { get; set; }
        public decimal? SickLeaveBalance { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string AgencyOfEmployment { get; set; }
        public bool? IsActive { get; set; }
        public bool IsEmployee { get; set; }
        public bool? ManateeAccess { get; set; }
        public string SarasotaAccess { get; set; }
        public string DesotoAccess { get; set; }
        public string SwnGroupID { get; set; }

        #region Extension Columns
        [IgnoreColumn]
        public string FullName
        {
            get
            {
                string lastname = LastName;
                if (!string.IsNullOrEmpty(MiddleInitial))
                {
                    lastname = string.Format("{0}. {1}", MiddleInitial, lastname);
                }
                return string.Format("{0} {1}", FirstName, lastname);
            }
        }
        [IgnoreColumn]
        public IEnumerable<Group> Groups { get { return GetEmployeeGroupMemberships(EmployeeId); } }
        [IgnoreColumn]
        public string DepartmentName { get { return GetEmployeeDepartment(DepartmentId?? -1); } }
        [IgnoreColumn]
        public IEnumerable<Phone> Phones { get { return GetEmployeePhones(EmployeeId); } }
        [IgnoreColumn]
        public IEnumerable<ServiceHistory> ServiceHistories { get { return GetEmployeeServiceHistory(SocialSecurityNumber); } }
        [IgnoreColumn]
        public IEnumerable<PositionHistory> PositionHistories { get { return GetEmployeePositionHistory(SocialSecurityNumber); } }
        [IgnoreColumn]
        public IEnumerable<EEO> EEOs { get { return GetEmployeeEEOs(JobGroupID ?? -1); } }
        [IgnoreColumn]
        public JobGroup JobCategory { get { return GetEmployeeJobGroup(JobGroupID ?? -1); } }
        [IgnoreColumn]
        public JobClass JobClass { get { return GetEmployeeJobClass(ClassId ?? -1); } }
        [IgnoreColumn]
        public OfficeLocation OfficeLocation { get { return GetEmployeeOfficeLocation(OfficeLocationId ?? -1); } }
        [IgnoreColumn]
        public string CountyName
        {
            get
            {
                var ctl = new CountyController(); 
                string countyName = ""; 
                if (CountyId.HasValue)
                {
                    County county = ctl.GetCounty(CountyId.Value); 
                    countyName = county.CountyName;
                }
                return countyName;
            }
        }
        #endregion

        #region Private Methods

        private string GetEmployeeDepartment(int departmentId)
        {
            var ctl = new GroupController();
            Group dept = ctl.GetGroup(departmentId);
            return dept != null ? dept.GroupName : "";
        }
        private IEnumerable<EEO> GetEmployeeEEOs(int jobGroupID)
        {
            var ctl = new EEOController();
            IEnumerable<EEO> eeos = ctl.GetEmployeeEEOs(jobGroupID);
            return eeos ?? eeos;
        }
        private JobGroup GetEmployeeJobGroup(int jobGroupID)
        {
            var ctl = new JobGroupController();
            JobGroup jobGroup = ctl.GetJobGroup(jobGroupID);
            return jobGroup ?? jobGroup;
        }
        private JobClass GetEmployeeJobClass(int ClassId)
        {
            var ctl = new JobClassController();
            JobClass jobClass = ctl.GetJobClass(ClassId);
            return jobClass ?? jobClass;
        }
        private OfficeLocation GetEmployeeOfficeLocation(int officeLocationId)
        {
            var ctl = new OfficeLocationController();
            OfficeLocation officeLocation = ctl.GetOfficeLocation(officeLocationId);
            return officeLocation ?? officeLocation;
        }
        private IEnumerable<Group> GetEmployeeGroupMemberships(int employeeId)
        {
            var ctl = new GroupController();
            IEnumerable<Group> groups = ctl.GetGroupMemberships(employeeId);
            return groups ?? groups;
        }
        private IEnumerable<Phone> GetEmployeePhones(int employeeId)
        {
            var ctl = new PhoneController();
            IEnumerable<Phone> phones = ctl.GetPhonesByEmployee(employeeId);
            return phones ?? phones;
        }
        private IEnumerable<ServiceHistory> GetEmployeeServiceHistory(string ssn)
        {
            var ctl = new ServiceHistoryController();
            IEnumerable<ServiceHistory> serviceHistories = ctl.GetServiceHistoriesByEmployee(ssn);
            return serviceHistories ?? serviceHistories;
        }
        private IEnumerable<PositionHistory> GetEmployeePositionHistory(string ssn)
        {
            var ctl = new PositionHistoryController();
            IEnumerable<PositionHistory> positionHistories = ctl.GetPositionHistoriesByEmployee(ssn);
            return positionHistories ?? positionHistories;
        }
        #endregion
    }
}
