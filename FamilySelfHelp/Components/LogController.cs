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

namespace tjc.Modules.FamilySelfHelp.Components
{
    internal class LogController
    {
        public Log CreateLog(Log t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                rep.Insert(t);
                return t;
            }
        }

        public void DeleteLog(long logId)
        {
            var t = GetLog(logId);
            DeleteLog(t);
        }

        public void DeleteLog(Log t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Log> GetLogs()
        {
            IEnumerable<Log> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Log> GetLogsByClient(long clientId)
        {
            IEnumerable<Log> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                t = rep.Find("Where ClientId = @0",clientId);
            }
            return t;
        }

        public Log GetLog(long logId)
        {
            Log t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                t = rep.GetById(logId);
            }
            return t;
        }

        public void UpdateLog(Log t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                rep.Update(t);
            }
        }
        public IEnumerable<Report> GetReport(DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
              return  ctx.ExecuteQuery<Report>(System.Data.CommandType.StoredProcedure, "tjc_shc_stat_report", startDate,endDate,string.Empty);
            }
        }
        public IEnumerable<Report> GetReport(DateTime startDate, DateTime endDate,string division)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Report>(System.Data.CommandType.StoredProcedure, "tjc_shc_stat_report", startDate, endDate,division);
            }
        }
        public IEnumerable<Report> GetCaseTypeReport(DateTime startDate, DateTime endDate, string division)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Report>(System.Data.CommandType.StoredProcedure, "tjc_shc_case_type_stat_report", startDate, endDate, division);
            }
        }
        public IEnumerable<Report> GetCaseTypeReport(DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Report>(System.Data.CommandType.StoredProcedure, "tjc_shc_case_type_stat_report", startDate, endDate, string.Empty);
            }
        }
        public IEnumerable<Report> GetServiceReport(DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Report>(System.Data.CommandType.StoredProcedure, "tjc_shc_service_stat_report", startDate, endDate, string.Empty);
            }
        }
        public IEnumerable<Report> GetServiceReport(DateTime startDate, DateTime endDate, string division)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Report>(System.Data.CommandType.StoredProcedure, "tjc_shc_service_stat_report", startDate, endDate, division);
            }
        }
        public void CreateCaseTypesByLog(IEnumerable<CaseType> caseTypes,long logid)
        {
            DeleteCaseTypesByLog(logid);
            using (IDataContext ctx = DataContext.Instance())
            {
                foreach (CaseType caseType in caseTypes)
                {
                    ctx.Execute(System.Data.CommandType.Text, "Insert Into tjc_shc_case_types (LogID,CaseTypeName) Values(@0, @1)", caseType.LogID,caseType.CaseTypeName);
                }
            }
        }
        public IEnumerable<CaseType> GetCaseTypesByLog(long logid)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<CaseType>(System.Data.CommandType.Text, "Select * from tjc_shc_case_types Where LogID=@0",logid);
            }
        }
        public void DeleteCaseTypesByLog(long logId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                 ctx.Execute(System.Data.CommandType.Text,"Delete from tjc_shc_case_types Where LogID=@0", logId);
            }

        }
        public void CreateServicesByLog(IEnumerable<Service> services, long logid)
        {
            DeleteServicesByLog(logid);
            using (IDataContext ctx = DataContext.Instance())
            {
                foreach (Service service in services)
                {
                    ctx.Execute(System.Data.CommandType.Text, "Insert Into tjc_shc_services (LogID,ServiceName) Values(@0, @1)", service.LogID, service.ServiceName);
                }
            }
        }
        public IEnumerable<Service> GetServicesByLog(long logid)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Service>(System.Data.CommandType.Text,"Select * from tjc_shc_services Where LogID=@0", logid);
            }
        }
        public void DeleteServicesByLog(long logId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.Text, "Delete from tjc_shc_services Where LogID=@0", logId);
            }

        }
    }
}
