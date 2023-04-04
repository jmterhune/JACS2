/*
' Copyright (c) 2022 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Intranet.API.Components.Employee
{
    internal class EmployeeController
    {
        private const string CONN_INTRANET = "Intranet"; //Connection
        public Employee GetEmployeePersonalInfo(string email)
        {
            Employee t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Employee>();
                t = rep.Find("Where EmailWork Like @0", email.Trim() + "%").FirstOrDefault();
            }
            return t;
        }
        public Employee GetEmployeeById(long employeeId)
        {
            Employee t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Employee>();
                t = rep.GetById(employeeId);
            }
            return t;
        }
        public Phone GetPhoneById(long phoneId)
        {
            Phone t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.GetById(phoneId);
            }
            return t;
        }
        public EmergencyContact GetEmergencyContactById(long employeeId)
        {
            EmergencyContact t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<EmergencyContact>();
                t = rep.GetById(employeeId);
            }
            return t;
        }
        public IEnumerable<Phone> GetEmployeePhones(long employeeId)
        {
            IEnumerable<Phone> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.Find("Where EmployeeId = @0", employeeId);
            }
            return t;
        }
        public IEnumerable<EmergencyContact> GetEmergencyContacts(long employeeId)
        {
            IEnumerable<EmergencyContact> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<EmergencyContact>();
                t = rep.Find("Where EmployeeId = @0", employeeId);
            }
            return t;
        }

        public bool UpdateEmployeePersonalData(Employee employee)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
                {
                    var rep = ctx.GetRepository<Employee>();
                    rep.Update(employee);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UpsertPhone(Phone phone)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
                {
                    if (phone.PhoneId == 0)
                    {
                        var rep = ctx.GetRepository<Phone>();
                        rep.Insert(phone);
                    }
                    else
                    {
                        var rep = ctx.GetRepository<Phone>();
                        rep.Update(phone);
                    }
                }
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }
        }
        public void DeletePhones(IEnumerable<long> phoneIds)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                foreach (long id in phoneIds)
                {
                    var rep = ctx.GetRepository<Phone>();
                    Phone phone = rep.GetById(id);
                    rep.Delete(phone);
                }
            }
        }
        public void UpsertEmergencyContacts(EmergencyContact contact)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
                {
                    if (contact.ContactId == 0)
                    {
                        var rep = ctx.GetRepository<EmergencyContact>();
                        rep.Insert(contact);
                    }
                    else
                    {
                        var rep = ctx.GetRepository<EmergencyContact>();
                        rep.Update(contact);
                    }
                }
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }
        }
        public void DeleteEmergencyContacts(IEnumerable<long> contactIds)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                foreach (long id in contactIds)
                {
                    var rep = ctx.GetRepository<EmergencyContact>();
                    EmergencyContact contact = rep.GetById(id);
                    rep.Delete(contact);
                }
            }
        }
    }
}
