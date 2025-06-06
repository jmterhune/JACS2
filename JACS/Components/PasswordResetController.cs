using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class PasswordResetController
    {
        public void CreatePasswordReset(PasswordReset t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PasswordReset>();
                rep.Insert(t);
            }
        }
        public void DeletePasswordReset(int passwordresetId)
        {
            var t = GetPasswordReset(passwordresetId);
            DeletePasswordReset(t);
        }
        public void DeletePasswordReset(PasswordReset t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PasswordReset>();
                rep.Delete(t);
            }
        }
        public IEnumerable<PasswordReset> GetPasswordResets()
        {
            IEnumerable<PasswordReset> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PasswordReset>();
                t = rep.Get();
            }
            return t;
        }
        public PasswordReset GetPasswordReset(int passwordresetId)
        {
            PasswordReset t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PasswordReset>();
                t = rep.GetById(passwordresetId);
            }
            return t;
        }
        public void UpdatePasswordReset(PasswordReset t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PasswordReset>();
                rep.Update(t);
            }
        }
    }
}