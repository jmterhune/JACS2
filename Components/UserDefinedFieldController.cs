using DotNetNuke.Data;
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
                var rep = ctx.GetRepository<UserDefinedField>();
                rep.Insert(t);
            }
        }
        public void DeleteUserDefinedField(int userdefinedfieldId)
        {
            var t = GetUserDefinedField(userdefinedfieldId);
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
        public UserDefinedField GetUserDefinedField(int userdefinedfieldId)
        {
            UserDefinedField t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<UserDefinedField>();
                t = rep.GetById(userdefinedfieldId);
            }
            return t;
        }
        public void UpdateUserDefinedField(UserDefinedField t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<UserDefinedField>();
                rep.Update(t);
            }
        }
    }
}
