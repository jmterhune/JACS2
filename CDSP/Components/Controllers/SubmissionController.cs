/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using tjc.Modules.CDSPAdmin.Components.Models;

namespace tjc.Modules.CDSPAdmin.Components.Controllers
{
    /// <summary>
    /// Data access for CDSP submissions. The records live in the
    /// jud12.flcourts.org database (where the public CDSP form writes them),
    /// so this controller uses the "Jud12" connection rather than the
    /// intranet site's default SiteSqlServer connection — same pattern the
    /// CourtRegistry module uses.
    /// </summary>
    public class SubmissionController
    {
        private const string CONN_JUD12 = "Jud12";

        public IEnumerable<SubmissionInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<SubmissionInfo>();
                return rep.Get();
            }
        }

        public SubmissionInfo GetSubmission(int submissionId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<SubmissionInfo>();
                return rep.GetById(submissionId);
            }
        }

        /// <summary>
        /// Targeted update of just the Completed flag plus the audit columns.
        /// Avoids round-tripping the whole row to flip one bit.
        /// </summary>
        public void SetCompleted(int submissionId, bool completed, int userId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(CommandType.Text,
                    "UPDATE tjc_cdsp_submission SET Completed = @0, LastModifiedDate = @1, LastModifiedById = @2 WHERE SubmissionID = @3",
                    completed, DateTime.Now, userId, submissionId);
            }
        }
    }
}
