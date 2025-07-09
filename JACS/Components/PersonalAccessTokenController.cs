using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class PersonalAccessTokenController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreatePersonalAccessToken(PersonalAccessToken t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<PersonalAccessToken>();
                rep.Insert(t);
            }
        }
        public void DeletePersonalAccessToken(int personalaccesstokenId)
        {
            var t = GetPersonalAccessToken(personalaccesstokenId);
            DeletePersonalAccessToken(t);
        }
        public void DeletePersonalAccessToken(PersonalAccessToken t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<PersonalAccessToken>();
                rep.Delete(t);
            }
        }
        public IEnumerable<PersonalAccessToken> GetPersonalAccessTokens()
        {
            IEnumerable<PersonalAccessToken> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<PersonalAccessToken>();
                t = rep.Get();
            }
            return t;
        }
        public PersonalAccessToken GetPersonalAccessToken(int personalaccesstokenId)
        {
            PersonalAccessToken t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<PersonalAccessToken>();
                t = rep.GetById(personalaccesstokenId);
            }
            return t;
        }
        public void UpdatePersonalAccessToken(PersonalAccessToken t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<PersonalAccessToken>();
                rep.Update(t);
            }
        }
    }
}