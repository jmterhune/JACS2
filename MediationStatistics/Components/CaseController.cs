using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class CaseController
    {
        public void CreateCase(Case t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Case>();
                rep.Insert(t);
            }
        }

        public void DeleteCase(int caseId)
        {
            var t = GetCase(caseId);
            DeleteCase(t);
        }

        public void DeleteCase(Case t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Case>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Case> GetCases()
        {
            IEnumerable<Case> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Case>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Case> GetExistingCase(string caseNumber, string cdspNumber)
        {
            IEnumerable<Case> t;
            if(!string.IsNullOrEmpty(caseNumber) && !string.IsNullOrEmpty(cdspNumber))
            {
                cdspNumber += "%";
                caseNumber += "%";
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<Case>();
                    t = rep.Find("Where CaseNumber like @0 or CDSPNumber like @1", caseNumber, cdspNumber);
                }
            }else if (!string.IsNullOrEmpty(caseNumber))
            {
                caseNumber += "%";
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<Case>();
                    t = rep.Find("Where CaseNumber like @0", caseNumber);
                }
            }
            else if (!string.IsNullOrEmpty(cdspNumber))
            {
                cdspNumber += "%";
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<Case>();
                    t = rep.Find("Where CDSPNumber like @0",  cdspNumber);
                }
            }
            else { return null; }
          
            return t;
        }

        public IEnumerable<CaseListItem> GetCaseList(int groupId, int regionId, string caseNumber, string cdspNumber, string firstName, string lastName,DateTime startDate,DateTime endDate)
        {
            IEnumerable<CaseListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                
                t = ctx.ExecuteQuery<CaseListItem>(System.Data.CommandType.StoredProcedure, "tjc_med_get_case_list", groupId, regionId, caseNumber, cdspNumber, firstName, lastName,startDate,endDate);
            }
            return t;
        }
        public IEnumerable<CaseListItem> GetCaseListPaged(int groupId, int regionId, string caseNumber, string cdspNumber, string firstName, string lastName,string businessName, int rowOffset,int pageSize,string SortOrder,bool sortDesc)
        {
            IEnumerable<CaseListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<CaseListItem>(System.Data.CommandType.StoredProcedure, "tjc_med_get_case_list_paged", groupId, regionId, caseNumber, cdspNumber, firstName, lastName, businessName, rowOffset, pageSize,SortOrder,sortDesc);
            }
            return t;
        }
        public int GetCaseListCount(int groupId, int regionId, string caseNumber, string cdspNumber, string firstName, string lastName, string businessName)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_med_get_case_list_count", groupId, regionId, caseNumber, cdspNumber, firstName, lastName,businessName);
            }
            return t;
        }


        public Case GetCase(int caseId)
        {
            Case t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Case>();
                t = rep.GetById(caseId);
            }
            return t;
        }

        public void UpdateCase(Case t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Case>();
                rep.Update(t);
            }
        }

    }
}
