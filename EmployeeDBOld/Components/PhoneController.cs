/*
' Copyright (c) 2023 Joe Terhune
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace tjc.Modules.EmployeeDB.Components
{
    internal class PhoneController
    {
        public void CreatePhone(Phone t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Phone>();
                rep.Insert(t);
            }
        }

        public void DeletePhone(int phoneId)
        {
            var t = GetPhone(phoneId);
            DeletePhone(t);
        }

        public void DeletePhone(Phone t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Phone>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Phone> GetPhones()
        {
            IEnumerable<Phone> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<PhoneListItem> GetPhoneListByEmployee(long employeeId)
        {
            IEnumerable<PhoneListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhoneListItem>();
                t = rep.Find("Where EmployeeId=@0", employeeId);
            }
            return t.OrderBy(x => x.PhoneCascade);
        }
        public IEnumerable<Phone> GetPhonesByEmployee(long employeeId)
        {
            IEnumerable<Phone> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.Find("Where EmployeeId=@0", employeeId);
            }
            return t;
        }
        public IEnumerable<SwnPhone> GetSwnPhonesByEmployee(long employeeId)
        {
            IEnumerable<SwnPhone> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<SwnPhone>(System.Data.CommandType.Text, "select p.*, '1' AS CountyCode,'SMS' as SmsLabel from tjc_employee_phone p Where EmployeeId=@0", employeeId);

            }
            return t;
        }
        public Phone GetPhone(int phoneId)
        {
            Phone t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.GetById(phoneId);
            }
            return t;
        }
        public int GetMaxPhone()
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "select Max(phoneCount) from (select Count(EmployeeID) as phoneCount from tjc_employee_phone Where SwnCall=1 Group by EmployeeId) t");
            }
            return phoneCount;
        }
        public int GetMaxPhoneCascade(int employeeId)
        {
            int maxCascade = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                maxCascade = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "select Max(PhoneCascade) from tjc_employee_phone Where (SwnCall = 1 Or SwnText = 1) And EmployeeId = @0", employeeId);
            }
            return maxCascade;
        }
        public int GetMaxSMS()
        {
            int smsCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                smsCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "select Max(textCount) from (select Count(EmployeeID) as textCount from tjc_employee_phone Where SwnText=1 Group by EmployeeId) t");
            }
            return smsCount;
        }
        public void UpdatePhone(Phone t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Phone>();
                rep.Update(t);
            }
        }
        public void FixPhoneSort(long employeeId)
        {
            IList<Phone> swnPhones = GetPhonesByEmployee(employeeId).Where(x => x.SwnCall == true | x.SwnText == true).OrderBy(x => x.PhoneCascade).ToList();
            int order = 0;
            foreach (Phone phone in swnPhones)
            {
                phone.PhoneCascade = order;
                order++;
                UpdatePhone(phone);
            }
        }
        public void MovePhoneCascade(long employeeId, int phoneId, string direction)
        {
            List<Phone> swnPhones = GetPhonesByEmployee(employeeId).Where(x => x.SwnCall == true | x.SwnText == true).OrderBy(x => x.PhoneCascade).ToList();
            var count = swnPhones.Count();
            Phone selectedPhone = swnPhones.Where(x => x.PhoneId == phoneId).FirstOrDefault();
            int itemIndex = swnPhones.FindIndex(a=>a.PhoneId==phoneId);
            var otherPhone = new Phone();
            int oldSort = 0;
            if (selectedPhone.PhoneCascade > 0 )
            {
                if (direction == "up")
                {
                    otherPhone = swnPhones.ElementAt(itemIndex - 1);
                }
                else
                {
                    otherPhone = swnPhones.ElementAt(itemIndex + 1);
                }
                oldSort = selectedPhone.PhoneCascade;
                selectedPhone.PhoneCascade = otherPhone.PhoneCascade;
                otherPhone.PhoneCascade = oldSort;
                UpdatePhone(selectedPhone);
                UpdatePhone(otherPhone);
            }
        }
    }
}
