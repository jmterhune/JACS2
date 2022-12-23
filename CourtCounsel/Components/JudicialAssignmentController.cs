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
using System.Linq;

namespace tjc.Modules.CourtCounsel.Components
{
    internal class JudicialAssignmentController
    {
        private const string CONN_INTRANET = "Intranet.API"; //Connection

        public void DeleteJudicialAssignment(long assignmentId, int judgeId)
        {
            var t = GetJudicialAssignment(assignmentId, judgeId);
            DeleteJudicialAssignment(t);
        }

        public void DeleteJudicialAssignment(JudicialAssignment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<JudicialAssignment>();
                rep.Delete(t);
            }
        }

        public IEnumerable<JudicialAssignment> GetJudicialAssignments()
        {
            IEnumerable<JudicialAssignment> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<JudicialAssignment>();
                t = rep.Get();
            }
            return t;
        }

        public JudicialAssignment GetJudicialAssignment(long assignmentId, int judgeId)
        {
            JudicialAssignment t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<JudicialAssignment>();
                t = rep.Find("Where AssignmentId = @0 And JudgeId = @1", assignmentId, judgeId).FirstOrDefault();
            }
            return t;
        }
        public void UpdateJudicialAssignment(JudicialAssignment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<JudicialAssignment>();
                rep.Update(t);
            }
        }

        public void CreateJudicialAssignment(JudicialAssignment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "court_counsel_add_judicial_assignment", t.AssignmentId,t.JudgeId,t.DateAssigned,t.DateRemoved,t.Reason,t.CreatedBy,t.ModifiedBy,t.CreatedDate,t.ModifiedDate);
            }
        }
    }
}
