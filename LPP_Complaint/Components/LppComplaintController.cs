/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.LPP_Complaint.Components
{
    /// <summary>
    /// Reads and writes the LPP complaints, which live in the public (jud12) database
    /// rather than this intranet site's own.
    /// </summary>
    public class LppComplaintController
    {
        /// <summary>Connection string to the public site's database, from web.config.</summary>
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public LppComplaintController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        /// <summary>
        /// Every complaint, newest first, each with its "Nature of Incident" rows attached.
        /// The natures are fetched once for the whole set rather than per complaint.
        /// </summary>
        public IEnumerable<LppComplaint> GetComplaints()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                List<LppComplaint> complaints = ctx.GetRepository<LppComplaint>()
                    .Get()
                    .OrderByDescending(c => c.LppComplaintId)
                    .ToList();

                var byComplaint = ctx.GetRepository<LppComplaintNature>()
                    .Get()
                    .GroupBy(n => n.LppComplaintId)
                    .ToDictionary(g => g.Key, g => (IList<string>)g.Select(n => n.Nature).ToList());

                foreach (LppComplaint complaint in complaints)
                {
                    IList<string> natures;
                    complaint.Natures = byComplaint.TryGetValue(complaint.LppComplaintId, out natures)
                        ? natures
                        : new List<string>();
                }

                return complaints;
            }
        }

        /// <summary>One complaint, with its nature rows loaded.</summary>
        public LppComplaint GetComplaint(int lppComplaintId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                LppComplaint t = ctx.GetRepository<LppComplaint>().GetById(lppComplaintId);
                if (t != null)
                {
                    t.Natures = ctx.GetRepository<LppComplaintNature>()
                        .Find("WHERE LppComplaintId = @0", lppComplaintId)
                        .Select(n => n.Nature)
                        .ToList();
                }
                return t;
            }
        }

        public void UpdateComplaint(LppComplaint t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<LppComplaint>().Update(t);
            }
        }

        /// <summary>
        /// Records who moved the complaint to a new status and when. Returns false when
        /// the complaint has since been deleted by someone else.
        /// </summary>
        public bool SetStatus(int lppComplaintId, string status, string notes, int userId)
        {
            LppComplaint complaint = GetComplaint(lppComplaintId);
            if (complaint == null) return false;

            complaint.Status = status;
            complaint.StatusNotes = notes;
            complaint.StatusChangedByUserId = userId;
            complaint.StatusChangedOnDate = DateTime.Now;
            UpdateComplaint(complaint);
            return true;
        }

        /// <summary>
        /// Removes the complaint. The nature rows go with it through the cascade on
        /// FK_tjc_modules_lpp_complaint_natures_Complaint, but they are deleted here as
        /// well so the behaviour does not depend on the constraint being present.
        /// </summary>
        public void DeleteComplaint(int lppComplaintId)
        {
            LppComplaint t = GetComplaint(lppComplaintId);
            if (t == null) return;

            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var natureRep = ctx.GetRepository<LppComplaintNature>();
                foreach (var nature in natureRep.Find("WHERE LppComplaintId = @0", lppComplaintId).ToList())
                {
                    natureRep.Delete(nature);
                }

                ctx.GetRepository<LppComplaint>().Delete(t);
            }
        }
    }
}
