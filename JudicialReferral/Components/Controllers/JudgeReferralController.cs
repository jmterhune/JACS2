using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.JudicialReferral.Components.Models;

namespace tjc.Modules.JudicialReferral.Components.Controllers
{
    public class JudgeReferralController
    {
        public JudgeReferralInfo GetReferral(int referralId)
        {
            JudgeReferralInfo item;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeReferralInfo>();
                item = rep.GetById(referralId);
            }
            return item;
        }

        public IEnumerable<JudgeReferralInfo> GetReferralsByJudge(int judgeId)
        {
            IEnumerable<JudgeReferralInfo> items;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeReferralInfo>();
                items = rep.Find("WHERE JudgeId = @0", judgeId);
            }
            return items;
        }

        public IEnumerable<JudgeReferralInfo> GetFilteredReferrals(DateTime? startDate, DateTime? endDate,
            string caseNumber, int judgeId, string motionTitle, int status)
        {
            IEnumerable<JudgeReferralInfo> items;
            using (IDataContext ctx = DataContext.Instance())
            {
                var conditions = new List<string>();
                var args = new List<object>();
                int paramIndex = 0;

                if (startDate.HasValue)
                {
                    conditions.Add(string.Format("JaCreatedDate >= @{0}", paramIndex++));
                    args.Add(startDate.Value.Date);
                }
                if (endDate.HasValue)
                {
                    conditions.Add(string.Format("JaCreatedDate <= @{0}", paramIndex++));
                    args.Add(endDate.Value.Date.AddDays(1));
                }
                if (!string.IsNullOrEmpty(caseNumber))
                {
                    conditions.Add(string.Format("CaseNumber LIKE @{0}", paramIndex++));
                    args.Add("%" + caseNumber + "%");
                }
                if (judgeId > 0)
                {
                    conditions.Add(string.Format("JudgeId = @{0}", paramIndex++));
                    args.Add(judgeId);
                }
                if (!string.IsNullOrEmpty(motionTitle))
                {
                    conditions.Add(string.Format("MotionTitle LIKE @{0}", paramIndex++));
                    args.Add("%" + motionTitle + "%");
                }
                if (status > 0)
                {
                    conditions.Add(string.Format("Status = @{0}", paramIndex++));
                    args.Add(status);
                }

                string sql = "SELECT * FROM tjc_jr_referrals";
                if (conditions.Any())
                    sql += " WHERE " + string.Join(" AND ", conditions);
                sql += " ORDER BY JaCreatedDate DESC";

                items = ctx.ExecuteQuery<JudgeReferralInfo>(System.Data.CommandType.Text, sql, args.ToArray());
            }
            return items;
        }

        public int AddReferral(JudgeReferralInfo item)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeReferralInfo>();
                rep.Insert(item);
            }
            return item.ReferralId;
        }

        public void UpdateReferral(JudgeReferralInfo item)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudgeReferralInfo>();
                rep.Update(item);
            }
        }

        public void UpdateStatus(int referralId, int status)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "UPDATE tjc_jr_referrals SET Status = @0 WHERE ReferralId = @1",
                    status, referralId);
            }
        }

        public void DeleteReferral(int referralId)
        {
            var item = GetReferral(referralId);
            if (item != null)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<JudgeReferralInfo>();
                    rep.Delete(item);
                }
            }
        }

        /// <summary>
        /// Emails Court Counsel notifying them that a referral has been routed to
        /// them. Pulled out of Review.ascx.cs so the in-place status editor on
        /// View.ascx (which goes through the WebAPI handler) can fire the same
        /// notification when an editor moves a referral to ReferredToCounsel.
        /// </summary>
        public static void SendToCounsel(JudgeReferralInfo objReferral, int portalId, int tabId, int moduleId, string courtCounselEmail)
        {
            if (objReferral == null) return;
            var objJudge = UserController.GetUserById(portalId, objReferral.JudgeId);
            if (objJudge == null) return;

            string editUrl = Globals.NavigateURL(tabId, "editlog",
                "mid=" + moduleId,
                "rid=" + objReferral.ReferralId);

            string subject = "Judicial Referral Request";
            string body = string.Format(
                "<p>Please review the <a href='{0}'>Judicial Referral Request</a> for case number {1}.</p>",
                editUrl, objReferral.CaseNumber);

            Mail.SendEmail(objJudge.Email, courtCounselEmail, subject, body);
        }
    }
}
