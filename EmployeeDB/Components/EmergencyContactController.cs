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
    internal class EmergencyContactController
    {
        public void CreateEmergencyContact(EmergencyContact t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContact>();
                rep.Insert(t);
            }
        }

        public void DeleteEmergencyContact(int emergencyContactId)
        {
            var t = GetEmergencyContact(emergencyContactId);
            DeleteEmergencyContact(t);
        }

        public void DeleteEmergencyContact(EmergencyContact t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContact>();
                rep.Delete(t);
            }
        }

        public IEnumerable<EmergencyContact> GetEmergencyContacts()
        {
            IEnumerable<EmergencyContact> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContact>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<EmergencyContact> GetEmergencyContactsByEmployee(int employeeId)
        {
            IEnumerable<EmergencyContact> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContact>();
                t = rep.Find("Where EmployeeId = @0",employeeId);
            }
            return t;
        }
        public EmergencyContact GetEmergencyContact(int emergencyContactId)
        {
            EmergencyContact t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContact>();
                t = rep.GetById(emergencyContactId);
            }
            return t;
        }

        public void UpdateEmergencyContact(EmergencyContact t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContact>();
                rep.Update(t);
            }
        }

    }
}
