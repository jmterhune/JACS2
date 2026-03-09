using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.jacs.Components
{
    internal class CourtroomController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateCourtroom(Courtroom t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteCourtroom(long courtroomId)
        {
            var t = GetCourtroom(courtroomId);
            DeleteCourtroom(t);
        }
        public void DeleteCourtroom(Courtroom t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Courtroom> GetCourtrooms()
        {
            IEnumerable<Courtroom> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                t = rep.Get();
            }
            return t;
        }
        public List<KeyValuePair<long, string>> GetCourtroomDropDownItems(string searchTerm)
        {
            try
            {
                // Normalize search term
                searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm.Trim();

                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Courtroom>();
                    var results = rep.Find("WHERE description LIKE @0", $"%{searchTerm}%")
                        .Select(c => new KeyValuePair<long, string>(c.id, c.description)).OrderBy(c=>c.Value).ToList();
                    return results ?? new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }
        public Courtroom GetCourtroom(long courtroomId)
        {
            Courtroom t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                t = rep.GetById(courtroomId);
            }
            return t;
        }
        public void UpdateCourtroom(Courtroom t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<Courtroom> GetCourtroomPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Courtroom>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_courtroom_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }
        public int GetCourtroomCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_courtroom_count",
                    searchTerm ?? string.Empty
                );
            }
        }

    }
}