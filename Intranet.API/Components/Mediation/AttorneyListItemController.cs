using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Intranet.API.Components.Mediation { 
    internal class AttorneyListItemController
    {
        public IEnumerable<AttorneyListItem> GetAttorneyListPaged(string firstName, string lastName, string firm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<AttorneyListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<AttorneyListItem>(System.Data.CommandType.StoredProcedure, "tjc_med_get_attorney_list_paged",  firstName, lastName, firm, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public int GetAttorneyListCount( string firstName, string lastName, string firm)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_med_get_attorney_list_count", firstName, lastName, firm);
            }
            return t;
        }
        public void CreateAttorney(AttorneyListItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttorneyListItem>();
                rep.Insert(t);
            }
        }
    }
}
