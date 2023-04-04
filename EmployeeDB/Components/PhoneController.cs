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
using System.Collections.Generic;

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
        public IEnumerable<Phone> GetPhonesByEmployee(int employeeId)
        {
            IEnumerable<Phone> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.Find("Where EmployeeId=@0",employeeId);
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

        public void UpdatePhone(Phone t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Phone>();
                rep.Update(t);
            }
        }

    }
}
