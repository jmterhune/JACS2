using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;
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
        #region Designation Attorneys References
        public void CreateDesignationAttorney(int designationId, int attorneyId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_rec_add_designation_attorney", designationId, attorneyId);
            }
        }
        public IEnumerable<DesignationAttorney> GetDesignationAttorneys(int designationId)
        {
            IEnumerable<DesignationAttorney> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<DesignationAttorney>(System.Data.CommandType.StoredProcedure, "tjc_rec_get_designation_attorneys", designationId);
            }
            return t;
        }
        public DesignationAttorney GetDesignationAttorney(int designationId, int attorneyId)
        {
            DesignationAttorney t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<DesignationAttorney>(System.Data.CommandType.StoredProcedure, "tjc_rec_get_designation_attorney", designationId, attorneyId);
            }
            return t;
        }
        public IEnumerable<NameMatchViewModel> GetMatchingNames(string lastName)
        {
            IEnumerable<NameMatchViewModel> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<NameMatchViewModel>(System.Data.CommandType.StoredProcedure, "tjc_rec_get_matching_names", lastName);
            }
            return t;
        }
        public void DeleteDesignationAttorney(int designationId, int attorneyId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_rec_delete_designation_attorney", designationId, attorneyId);
            }
        }
        public void DeleteDesignationAttorneys(int designationId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_rec_delete_designation_attorney_all", designationId);
            }
        }
        #endregion
    }
}
