using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class ApiLogController
    {
        public void CreateApiLog(ApiLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ApiLog>();
                rep.Insert(t);
            }
        }
        public void DeleteApiLog(int apilogId)
        {
            var t = GetApiLog(apilogId);
            DeleteApiLog(t);
        }
        public void DeleteApiLog(ApiLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ApiLog>();
                rep.Delete(t);
            }
        }
        public IEnumerable<ApiLog> GetApiLogs()
        {
            IEnumerable<ApiLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ApiLog>();
                t = rep.Get();
            }
            return t;
        }
        public ApiLog GetApiLog(int apilogId)
        {
            ApiLog t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ApiLog>();
                t = rep.GetById(apilogId);
            }
            return t;
        }
        public void UpdateApiLog(ApiLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ApiLog>();
                rep.Update(t);
            }
        }
    }
}
