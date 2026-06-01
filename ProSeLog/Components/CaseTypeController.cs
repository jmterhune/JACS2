using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.ProSeLog.Components
{
    internal class CaseTypeController
    {
        public void CreateCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Insert(t);
            }
        }

        public void DeleteCaseType(int caseTypeId)
        {
            var t = GetCaseType(caseTypeId);
            DeleteCaseType(t);
        }

        public void DeleteCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CaseType> GetCaseTypes()
        {
            IEnumerable<CaseType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                t = rep.Get();
            }
            return t;
        }

        public CaseType GetCaseType(int caseTypeId)
        {
            CaseType t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                t = rep.GetById(caseTypeId);
            }
            return t;
        }

        public void UpdateCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Update(t);
            }
        }
        public IEnumerable<CaseNumber> GetCaseNumbers(string caseNumber)
        {
            IEnumerable<CaseNumber> t;
            caseNumber = string.Format("%{0}%", caseNumber);
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseNumber>();
                t = rep.Find("Where TEXT LIKE @0", caseNumber).Take(10);
                return t;
            }
        }

    }
}
