using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Intranet.API.Components.Mediation { 
    internal class MediatorListItemController
    {
        public IEnumerable<MediatorListItem> GetMediatorListPaged(string firstName, string lastName, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<MediatorListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<MediatorListItem>(System.Data.CommandType.StoredProcedure, "tjc_med_get_mediator_list_paged",  firstName, lastName, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public int GetMediatorListCount( string firstName, string lastName)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_med_get_mediator_list_count", firstName, lastName);
            }
            return t;
        }
        public void CreateMediator(MediatorListItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediatorListItem>();
                rep.Insert(t);
            }
        }
    }
}
