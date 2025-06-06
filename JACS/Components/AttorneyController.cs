using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class AttorneyController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                if (!t.scheduling.HasValue)
                    t.scheduling = false;
                if (!t.enabled.HasValue)
                    t.enabled = false;
                rep.Insert(t);
                if(t.emails != null && t.emails.Count > 0)
                {
                    var emailCtl = new EmailController();
                    foreach (var email in t.emails)
                    {
                        emailCtl.CreateEmail(new Email { emailable_id = t.id, emailable_type= "App\\Models\\Attorney", email = email });
                    }
                }
            }
        }
        public void DeleteAttorney(long attorneyId)
        {
            var t = GetAttorney(attorneyId);
            DeleteAttorney(t);
        }
        public void DeleteAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Attorney> GetAttorneys()
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Get();
            }
            return t;
        }
        public Attorney GetAttorney(long attorneyId)
        {
            Attorney t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.GetById(attorneyId);
            }
            return t;
        }
        public void UpdateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
                if(t.emails != null && t.emails.Count > 0)
                {
                    var emailCtl = new EmailController();
                    emailCtl.DeletAllEmailsByAttorney(t.id);
                    foreach (var email in t.emails)
                    {
                        emailCtl.CreateEmail(new Email { emailable_id = t.id, emailable_type= "App\\Models\\Attorney", email = email });
                    }
                }
            }
        }
        public IEnumerable<Attorney> GetAttorneysPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.ExecuteQuery<Attorney>(System.Data.CommandType.StoredProcedure, "tjc_jacs_get_attorney_paged", searchTerm, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public int GetAttorneysCount(string searchTerm)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_jacs_get_attorney_count", searchTerm);
            }
            return t;
        }
    }
}