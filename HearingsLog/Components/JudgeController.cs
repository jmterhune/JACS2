using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.HearingLog.Components
{
    internal class JudgeController
    {
        private readonly IHostSettings _hostSettings;

        public JudgeController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateJudge(JacsJudge t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<JacsJudge>();
                rep.Insert(t);
            }
        }

        public void DeleteJudge(int judgeId)
        {
            var t = GetJudge(judgeId);
            DeleteJudge(t);
        }

        public void DeleteJudge(JacsJudge t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<JacsJudge>();
                rep.Delete(t);
            }
        }

        public IEnumerable<JacsJudge> GetJudges()
        {
            IEnumerable<JacsJudge> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<JacsJudge>();
                t = rep.Get();
            }
            return t;
        }

        public JacsJudge GetJudge(int judgeId)
        {
            JacsJudge t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<JacsJudge>();
                t = rep.GetById(judgeId);
            }
            return t;
        }

        public void UpdateJudge(JacsJudge t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<JacsJudge>();
                rep.Update(t);
            }
        }
        public void CreateJacsJudgeByUserRef(int jacsUserId, int userId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_hearing_create_jacs_judge_by_user_ref", jacsUserId, userId);
            }
        }
        public void DeleteJacsJudgeByUserRef(int jacsUserId, int userId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_hearing_delete_jacs_judge_by_user_ref", jacsUserId, userId);
            }
        }
        public void DeleteJacsJudgesByUserRef(int userId, string county)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_hearing_delete_jacs_judges_by_user_ref", userId,county);
            }
        }
        public IEnumerable<JacsJudge> GetJacsJudgeByUserRef(int userId,string county)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.ExecuteQuery<JacsJudge>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_user_jacs_judges", userId,county);
            }
        }
        public IEnumerable<JacsJudge> GetJacsJudgeByCounty(string county)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.ExecuteQuery<JacsJudge>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_county_jacs_judges",  county);
            }
        }
        public void CreateJudgeJaRef(int judgeUserId, int jaUserId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_hearing_create_judge_ja_ref", judgeUserId, jaUserId);
            }
        }
        public void DeleteJudgeJaRef(int judgeUserId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_hearing_delete_judge_ja_ref", judgeUserId);
            }
        }

        public JudgeJa GetJudgeJaRef(int judgeUserId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var judges= ctx.ExecuteQuery<JudgeJa>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_judge_ja_ref", judgeUserId);
                return judges.FirstOrDefault();
            }
        }
        public JudgeJa GetJaJudgeRef(int jaUserId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var judges = ctx.ExecuteQuery<JudgeJa>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_ja_judge_ref", jaUserId);
                return judges.FirstOrDefault();
            }
        }
        public IEnumerable<JudgeJa> ListJudgeJaRef()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.ExecuteQuery<JudgeJa>(System.Data.CommandType.StoredProcedure, "tjc_hearing_list_judge_ja_ref");
            }
        }
        public IEnumerable<ExistingJacsJudges> GetExistingJacsJudges(string county, int userId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.ExecuteQuery<ExistingJacsJudges>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_existing_county_jacs_judges", county,userId);
            }
        }
    }
}
