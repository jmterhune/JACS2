using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class DesignationController
    {
        public void CreateDesignation(Designation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                rep.Insert(t);
            }
        }
        public void DeleteDesignation(int designationId)
        {
            var t = GetDesignation(designationId);
            DeleteDesignation(t);
        }
        public void DeleteDesignation(Designation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Designation> GetDesignations()
        {
            IEnumerable<Designation> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                t = rep.Find("WHERE Archived <> 1 OR Archived IS NULL").OrderByDescending(x=>x.DesignationID);
            }
            return t;
        }
        public IEnumerable<DesignationListItem> GetDesignationListPaged(string lastName, string firstName, string caseNumber, string county, bool archived, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<DesignationListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<DesignationListItem>(System.Data.CommandType.StoredProcedure, "tjc_rec_get_designation_list_paged", firstName, lastName,caseNumber,county,archived, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public int GetDesignationListCount(string lastName, string firstName, string caseNumber, string county, bool archived)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_rec_get_designation_list_count",  lastName, firstName,  caseNumber,  county,  archived);
            }
            return t;
        }
        public Designation GetDesignation(int designationId)
        {
            Designation t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                t = rep.GetById(designationId);
            }
            return t;
        }
        public void UpdateDesignation(Designation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                rep.Update(t);
            }
        }
        public void ToggleArchiveStatus(int designationId)
        {
            Designation t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                t=rep.GetById(designationId);
                t.Archived = !t.Archived;
                rep.Update(t);
            }
        }
        public void ToggleAcknowledgmentStatus(int designationId)
        {
            Designation t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Designation>();
                t = rep.GetById(designationId);
                t.AcknowledgmentFiled = !t.AcknowledgmentFiled;
                rep.Update(t);
            }
        }
    }
}
