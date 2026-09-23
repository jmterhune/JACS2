using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    // Read-only access to the global tjc_gl_counties table.
    public class CountyController
    {
        private readonly IHostSettings _hostSettings;

        public CountyController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<CountyInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<CountyInfo>();
                return rep.Get();
            }
        }

        public CountyInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<CountyInfo>();
                return rep.GetById(id);
            }
        }

        public CountyInfo GetByName(string name)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<CountyInfo>();
                return rep.Find("WHERE CountyName = @0", name).FirstOrDefault();
            }
        }
    }
}
