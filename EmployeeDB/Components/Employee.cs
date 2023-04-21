using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Web.Caching;

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

        public int? JobGroupId { get; set; }

        public int? ClassId { get; set; }
        public int? OfficeLocationId { get; set; }
        public int? CountyId { get; set; }
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

        public string SwnGroupId { get; set; }

        #region Extension Columns
        [IgnoreColumn]
        public IEnumerable<Group> Groups { get { return GetEmployeeGroupMemberships(EmployeeId); } }
        [IgnoreColumn]
        public string DepartmentName { get { return GetEmployeeDepartment(DepartmentId ?? 0); } }
        [IgnoreColumn]
        public IEnumerable<Phone> Phones { get { return GetEmployeePhones(EmployeeId); } }
        [IgnoreColumn]
        public IEnumerable<ServiceHistory> ServiceHistories { get { return GetEmployeeServiceHistory(SocialSecurityNumber); } }
        [IgnoreColumn]
        public IEnumerable<PositionHistory> PositionHistories { get { return GetEmployeePositionHistory(SocialSecurityNumber); } }
        [IgnoreColumn]
        public IEnumerable<EEO> EEOs { get { return GetEmployeeEEOs(JobGroupId ?? 0); } }
        [IgnoreColumn]
        public JobGroup JobCategory { get { return GetEmployeeJobGroup(JobGroupId ?? 0); } }
        [IgnoreColumn]
        public JobClass JobClass { get { return GetEmployeeJobClass(ClassId ?? 0); } }
        [IgnoreColumn]
        public OfficeLocation OfficeLocation { get { return GetEmployeeOfficeLocation(OfficeLocationId ?? 0); } }

        #endregion

        #region Private Methods

        private string GetEmployeeDepartment(int departmentId)
        {
            var ctl = new GroupController();
            Group dept = ctl.GetGroup(departmentId);
            return dept != null ? dept.GroupName : "";
        }
        private IEnumerable<EEO> GetEmployeeEEOs(int jobGroupId)
        {
            var ctl = new EEOController();
            IEnumerable<EEO> eeos = ctl.GetEmployeeEEOs(jobGroupId);
            return eeos ?? eeos;
        }
        private JobGroup GetEmployeeJobGroup(int jobGroupId)
        {
            var ctl = new JobGroupController();
            JobGroup jobGroup = ctl.GetJobGroup(jobGroupId);
            return jobGroup?? jobGroup;
        }
        private JobClass GetEmployeeJobClass(int ClassId)
        {
            var ctl = new JobClassController();
            JobClass jobClass = ctl.GetJobClass(ClassId);
            return jobClass?? jobClass;
        }
        private OfficeLocation GetEmployeeOfficeLocation(int officeLocationId)
        {
            var ctl = new OfficeLocationController();
            OfficeLocation officeLocation= ctl.GetOfficeLocation(officeLocationId);
            return officeLocation ?? officeLocation;
        }
        private IEnumerable<Group> GetEmployeeGroupMemberships(int employeeId)
        {
            var ctl = new GroupController();
            IEnumerable<Group> groups = ctl.GetGroupMemberships(employeeId);
            return groups?? groups;
        }
        private IEnumerable<Phone> GetEmployeePhones(int employeeId)
        {
            var ctl = new PhoneController();
            IEnumerable<Phone> phones = ctl.GetPhonesByEmployee(employeeId);
            return phones?? phones;
        }
        private IEnumerable<ServiceHistory> GetEmployeeServiceHistory(string ssn)
        {
            var ctl = new ServiceHistoryController();
            IEnumerable<ServiceHistory> serviceHistories= ctl.GetServiceHistoriesByEmployee(ssn);
            return serviceHistories?? serviceHistories;
        }
        private IEnumerable<PositionHistory> GetEmployeePositionHistory(string ssn)
        {
            var ctl = new PositionHistoryController();
            IEnumerable<PositionHistory> positionHistories= ctl.GetPositionHistoriesByEmployee(ssn);
            return positionHistories?? positionHistories;
        }
        #endregion
    }
}
