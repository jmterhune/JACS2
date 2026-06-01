using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Helpers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class SwnInterfaceLogController
    {
        public SwnInterfaceLogInfo GetById(long id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnInterfaceLogInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<SwnInterfaceLogInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT * FROM tjc_employee_swn_interface_log ORDER BY CreatedDate DESC";
                return ctx.ExecuteQuery<SwnInterfaceLogInfo>(CommandType.Text, sql);
            }
        }

        public long Create(SwnInterfaceLogInfo item)
        {
            ModelNormalizer.Normalize(item);
            if (!item.CreatedDate.HasValue)
                item.CreatedDate = DateTime.Now;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnInterfaceLogInfo>();
                rep.Insert(item);
            }
            return item.LogId;
        }

        public void Update(SwnInterfaceLogInfo item)
        {
            ModelNormalizer.Normalize(item);
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnInterfaceLogInfo>();
                rep.Update(item);
            }
        }

        public void Delete(long id)
        {
            var item = GetById(id);
            if (item != null)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<SwnInterfaceLogInfo>();
                    rep.Delete(item);
                }
            }
        }

        public long LogProcess(string process, string exception, int? userId)
        {
            var item = new SwnInterfaceLogInfo
            {
                Process = process,
                Exception = exception,
                CreatedDate = DateTime.Now,
                CreatedBy = userId
            };
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SwnInterfaceLogInfo>();
                rep.Insert(item);
            }
            return item.LogId;
        }

        public IEnumerable<SwnInterfaceLogInfo> GetRecentLogs(int topN)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT TOP " + topN + " * FROM tjc_employee_swn_interface_log ORDER BY CreatedDate DESC";
                return ctx.ExecuteQuery<SwnInterfaceLogInfo>(CommandType.Text, sql);
            }
        }
    }
}
