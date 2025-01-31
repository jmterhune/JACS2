/*
' Copyright (c) 2022 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.JudicialReferral.Components
{
    public class JudicialReferralController
    {
        public void CreateReferral(JudicialReferral t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudicialReferral>();
                rep.Insert(t);
            }
        }

        public void DeleteReferral(int referralId)
        {
            var t = GetReferral(referralId);
            DeleteReferral(t);
        }

        public void DeleteReferral(JudicialReferral t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudicialReferral>();
                rep.Delete(t);
            }
        }

        public IEnumerable<JudicialReferral> GetReferrals()
        {
            IEnumerable<JudicialReferral> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudicialReferral>();
                t = rep.Get();
            }
            return t;
        }
       
        public JudicialReferral GetReferral(int referralId)
        {
            JudicialReferral t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudicialReferral>();
                t = rep.GetById(referralId);
            }
            return t;
        }
        public IEnumerable<JudicialReferral> GetReferralList()
        {
            IEnumerable<JudicialReferral> t;

            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudicialReferral>();
                t = rep.Find("Where status=2 AND CounselReceivedDate IS NULL");
            }

            return t;
        }
        public IEnumerable<JudicialReferral> GetReferralList(int judgeId)
        {
            IEnumerable<JudicialReferral> t;

            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<JudicialReferral>(System.Data.CommandType.StoredProcedure, "tjc_judicial_referral_list_judge", judgeId);
            }

            return t;
        }
        public IEnumerable<JudicialReferral> GetReferralList(DateTime startDate, DateTime endDate, string caseNumber, int judgeId, string motionTitle, int status)
        {
            IEnumerable<JudicialReferral> t;

            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<JudicialReferral>(System.Data.CommandType.StoredProcedure, "tjc_judicial_referral_list_filtered", startDate, endDate, caseNumber, judgeId, motionTitle, status);
            }

            return t;
        }
        public void UpdateReferral(JudicialReferral t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JudicialReferral>();
                rep.Update(t);
            }
        }
        public IEnumerable<Attachment> GetReferralAttachments(int referralId)
        {
            IEnumerable<Attachment> t;

            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<Attachment>(System.Data.CommandType.StoredProcedure, "tjc_judicial_referral_get_referral_attachments", referralId);
            }

            return t;
        }
        public void DeleteReferralAttachments(int referralId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                 ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_judicial_referral_delete_referral_attachments", referralId);
            }

        }

        public void UpdateCourtCounsel(int referralId,DateTime counselReceivedDate,int status)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_judicial_referral_update_counsel", referralId,counselReceivedDate,status);
            }

        }
    }
}
