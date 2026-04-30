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
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.EmployeeDB.Components
{
    internal class SwnLogController
    {
        public void CreateSwnLog(SwnLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnLog>();
                rep.Insert(t);
            }
        }

        public void DeleteSwnLog(int logId)
        {
            var t = GetSwnLog(logId);
            DeleteSwnLog(t);
        }

        public void DeleteSwnLog(SwnLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnLog>();
                rep.Delete(t);
            }
        }

        public IEnumerable<SwnLog> GetSwnLogs()
        {
            IEnumerable<SwnLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnLog>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<SwnLogListItem> GetSwnLogList()
        {
            IEnumerable<SwnLogListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnLogListItem>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<SwnLogListItem> GetSwnLogList(DateTime startDate,DateTime endDate)
        {
            IEnumerable<SwnLogListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnLogListItem>();
                t = rep.Find("Where CreatedDate Between @0 And @1",startDate,endDate).OrderByDescending(x=>x.LogId);
            }
            return t;
        }

        public SwnLog GetSwnLog(int logId)
        {
            SwnLog t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnLog>();
                t = rep.GetById(logId);
            }
            return t;
        }

        public void UpdateSwnLog(SwnLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnLog>();
                rep.Update(t);
            }
        }
        public void ClearLog()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.Text, "Delete From tjc_employee_swn_interface_log");
            }
        }
    }
}
