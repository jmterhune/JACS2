using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Intranet.API.Components.Employee;
using tjc.Intranet.API.Services.ViewModels.Employee;

namespace tjc.Intranet.API.Services
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class EmployeeController : DnnApiController
    {
        [HttpGet]
        [AllowAnonymous]
        [ActionName("me")]
        public HttpResponseMessage GetEmployeePersonalDataByEmail(string emailAddress)
        {
            var ctl = new Components.Employee.EmployeeController();
            ViewModels.Employee.EmployeeViewModel employeeData = new ViewModels.Employee.EmployeeViewModel(ctl.GetEmployeePersonalInfo(emailAddress));
            if (employeeData == null) { employeeData = new ViewModels.Employee.EmployeeViewModel(); }
            return Request.CreateResponse(employeeData);
        }
        [HttpGet]
        [AllowAnonymous]
        [ActionName("contacts")]
        public HttpResponseMessage GetEmergencyContacts(long employeeId)
        {
            List<ViewModels.Employee.EmergencyContactViewModel> contacts = new List<ViewModels.Employee.EmergencyContactViewModel>();
            var ctl = new Components.Employee.EmployeeController();
            contacts = ctl.GetEmergencyContacts(employeeId).Select(contact => new ViewModels.Employee.EmergencyContactViewModel(contact)).ToList();
            return Request.CreateResponse(contacts);
        }
        [HttpGet]
        [AllowAnonymous]
        [ActionName("phones")]
        public HttpResponseMessage GetEmployeePhones(long employeeId)
        {
            List<ViewModels.Employee.PhoneViewModel> phones = new List<ViewModels.Employee.PhoneViewModel>();
            var ctl = new Components.Employee.EmployeeController();
            phones = ctl.GetEmployeePhones(employeeId).Select(phone => new ViewModels.Employee.PhoneViewModel(phone)).ToList();
            return Request.CreateResponse(phones);
        }
        [HttpPut]
        [AllowAnonymous]
        [ActionName("update-personal")]
        public HttpResponseMessage UpdatePersonalData(EmployeeViewModel employee)
        {
            var ctl = new Components.Employee.EmployeeController();
            try
            {
                Employee emp = ctl.GetEmployeeById(employee.EmployeeId);
                emp.EmailHome = employee.EmailHome;
                emp.Address1 = employee.Address1;
                emp.Address2 = employee.Address2;
                emp.City = employee.City;
                emp.State = employee.State;
                emp.Zip = employee.Zip;
                emp.Location = employee.Location;
                bool result = ctl.UpdateEmployeePersonalData(emp);
                if (!result)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                }
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ActionName("update-phones")]
        public HttpResponseMessage UpdatePhones(IEnumerable<PhoneViewModel> phones)
        {
            var ctl = new Components.Employee.EmployeeController();
            IEnumerable<long> oldPhonesList = ctl.GetEmployeePhones(phones.First().EmployeeId).Select(p => p.PhoneId);
            IEnumerable<long> newPhoneList = phones.Where(p => p.PhoneId > 0).Select(p => p.PhoneId);
            IEnumerable<long> missingPhoneIds = oldPhonesList.Where(p => newPhoneList.All(p2 => p2 != p));
            ctl.DeletePhones(missingPhoneIds);
            try
            {
                foreach (PhoneViewModel phone in phones)
                {
                    Phone p = MapPhone(phone);
                    ctl.UpsertPhone(p);

                }
                return Request.CreateResponse(System.Net.HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ActionName("update-contacts")]
        public HttpResponseMessage UpdateEmergencyContact(IEnumerable<EmergencyContactViewModel> contacts)
        {
            var ctl = new Components.Employee.EmployeeController();
            IEnumerable<long> oldContactList = ctl.GetEmergencyContacts(contacts.First().EmployeeId).Select(c => c.ContactId);
            IEnumerable<long> newContactList = contacts.Where(c => c.ContactId > 0).Select(c => c.ContactId);
            IEnumerable<long> missingContactIds = oldContactList.Where(c => newContactList.All(c2 => c2 != c));
            ctl.DeleteEmergencyContacts(missingContactIds);
            try
            {
                foreach (EmergencyContactViewModel contact in contacts)
                {
                    EmergencyContact c = MapEmergencyContacts(contact);
                    ctl.UpsertEmergencyContacts(c);

                }
                return Request.CreateResponse(System.Net.HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private Phone MapPhone(PhoneViewModel phone)
        {
            var ctl = new Components.Employee.EmployeeController();
            Phone p;
            if (phone.PhoneId == 0)
            {
                p = new Phone
                {
                    EmployeeId = phone.EmployeeId,
                    SWNCall = true,
                    SWNText = true,
                    SWNExcludeExtension = false,
                };
            }
            else
            {
                p = ctl.GetPhoneById(phone.PhoneId);
            }
            if (p != null)
            {
                p.PhoneNumber = phone.PhoneNumber;
                p.Extension = phone.Extension;
                p.PhoneType = phone.PhoneType;
                p.Location = phone.Location;
            }
            return p;
        }
        private EmergencyContact MapEmergencyContacts(EmergencyContactViewModel contact)
        {
            var ctl = new Components.Employee.EmployeeController();
            EmergencyContact c;
            if (contact.ContactId == 0)
            {
                c = new EmergencyContact
                {
                    EmployeeId = contact.EmployeeId,
                };
            }
            else
            {
                c = ctl.GetEmergencyContactById(contact.ContactId);
            }
            if (c != null)
            {
                c.CallOrder = contact.CallOrder;
                c.FirstName = contact.FirstName;
                c.LastName = contact.LastName;
                c.Relationship = contact.Relationship;
                c.PhoneWork = contact.PhoneWork;
                c.PhoneHome = contact.PhoneHome;
                c.PhoneMobile = contact.PhoneMobile;
            }
            return c;
        }
    }
}