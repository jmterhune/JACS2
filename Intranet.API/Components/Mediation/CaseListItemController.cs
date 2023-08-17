using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Intranet.API.Components.Mediation
{
    internal class CaseListItemController
    {
       
        public IEnumerable<CaseListItem> GetCaseList(int groupId, int regionId, string caseNumber, string cdspNumber, string firstName, string lastName,DateTime startDate,DateTime endDate)
        {
            IEnumerable<CaseListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                
                t = ctx.ExecuteQuery<CaseListItem>(System.Data.CommandType.StoredProcedure, "tjc_med_get_case_list", groupId, regionId, caseNumber, cdspNumber, firstName, lastName,startDate,endDate);
            }
            return t;
        }
        public IEnumerable<CaseListItem> GetCaseListPaged(int groupId, int regionId, string caseNumber, string cdspNumber, string firstName, string lastName,string businessName, int rowOffset,int pageSize,string sortOrder,string sortDesc)
        {
            IEnumerable<CaseListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<CaseListItem>(System.Data.CommandType.StoredProcedure, "tjc_med_get_case_list_paged", groupId, regionId, caseNumber, cdspNumber, firstName, lastName, businessName, rowOffset, pageSize,sortOrder,sortDesc);
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
        public void DeleteCase(int caseId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_delete_case", caseId);
            }
        }
    }
}
