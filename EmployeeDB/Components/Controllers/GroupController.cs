using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    // Read-only access to the global tjc_gl_group table.
    public class GroupController
    {
        public IEnumerable<GroupInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                return rep.Get();
            }
        }

        public GroupInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<GroupInfo> GetSwnGroups()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<GroupInfo>();
                return rep.Find("WHERE IsSwnGroup = 1");
            }
        }
    }
}
