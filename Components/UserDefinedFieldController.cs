using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.jacs.Components
{
    internal class UserDefinedFieldController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateUserDefinedField(UserDefinedField t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<UserDefinedField>();
                rep.Insert(t);
            }
        }
        public void DeleteUserDefinedField(long userDefinedFieldId)
        {
            var t = GetUserDefinedField(userDefinedFieldId);
            DeleteUserDefinedField(t);
        }
        public void DeleteUserDefinedField(UserDefinedField t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<UserDefinedField>();
                rep.Delete(t);
            }
        }
        public IEnumerable<UserDefinedField> GetUserDefinedFields()
        {
            IEnumerable<UserDefinedField> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<UserDefinedField>();
                t = rep.Get();
            }
            return t;
        }
        public UserDefinedField GetUserDefinedField(long userDefinedFieldId)
        {
            UserDefinedField t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<UserDefinedField>();
                t = rep.GetById(userDefinedFieldId);
            }
            return t;
        }
        public void UpdateUserDefinedField(UserDefinedField t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<UserDefinedField>();
                rep.Update(t);
            }
        }

        public IEnumerable<UserDefinedField> GetUserDefinedFieldsPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc, long courtId = 0)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<UserDefinedField>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_user_defined_field_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "field_name",
                    sortDesc ?? "asc",
                    courtId
                );
            }
        }
        public int GetUserDefinedFieldsCount(string searchTerm, long courtId = 0)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_user_defined_field_count",
                    searchTerm ?? string.Empty,
                    courtId
                );
            }
        }
    }
}