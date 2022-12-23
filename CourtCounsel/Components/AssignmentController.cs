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

namespace tjc.Modules.CourtCounsel.Components
{
    internal class AssignmentController 
    {
        private const string CONN_INTRANET = "Intranet.API"; //Connection

        public void CreateAssignment(Assignment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Assignment>();
                rep.Insert(t);
            }
        }

        public void DeleteAssignment(long assignmentId)
        {
            var t = GetAssignment(assignmentId);
            DeleteAssignment(t);
        }

        public void DeleteAssignment(Assignment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Assignment>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Assignment> GetAssignments()
        {
            IEnumerable<Assignment> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Assignment>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Assignment> GetAssignmentPage(int pageIndex,int pageSize)
        {
            IEnumerable<Assignment> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Assignment>();
                t = rep.GetPage(pageIndex,pageSize);
            }
            return t;
        }
        public long GetCurrentJudicialAssignment(long assignmentId)
        {
            Assignment t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Assignment>();
                t = rep.GetById(assignmentId);
            }
            return t.CurrentJudiciaryId;
        }
        public Assignment GetAssignment(long assignmentId)
        {
            Assignment t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Assignment>();
                t = rep.GetById(assignmentId);
            }
            return t;
        }
        public void UpdateAssignment(Assignment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Assignment>();
                rep.Update(t);
            }
        }
        public IEnumerable<Assignment> GetPendingAssignmentsToUpdate()
        {
            IEnumerable<Assignment> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Assignment>();
                t = rep.Find("Where StatusTypeId=1 And DateReceived<=@0",DateTime.Now);
                
            }
            return t;
        }
    }
}
