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
    internal class ServiceHistoryController
    {
        public void CreateServiceHistory(ServiceHistory t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistory>();
                rep.Insert(t);
            }
        }

        public void DeleteServiceHistory(int serviceHistoryId)
        {
            var t = GetServiceHistory(serviceHistoryId);
            DeleteServiceHistory(t);
        }

        public void DeleteServiceHistory(ServiceHistory t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistory>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ServiceHistory> GetServiceHistories()
        {
            IEnumerable<ServiceHistory> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistory>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<ServiceHistory> GetServiceHistoriesByEmployee(string ssn)
        {
            IEnumerable<ServiceHistory> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistory>();
                t = rep.Find("Where SocialSecurityNumber = @0",ssn);
            }
            return t;
        }

        public ServiceHistory GetServiceHistory(int serviceHistoryId)
        {
            ServiceHistory t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistory>();
                t = rep.GetById(serviceHistoryId);
            }
            return t;
        }

        public void UpdateServiceHistory(ServiceHistory t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistory>();
                rep.Update(t);
            }
        }

    }
}
