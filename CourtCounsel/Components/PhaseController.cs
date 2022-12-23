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
    internal class PhaseController
    {
        private const string CONN_INTRANET = "Intranet.API"; //Connection

        public void CreatePhase(Phase t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phase>();
                rep.Insert(t);
            }
        }

        public void DeletePhase(int phaseId)
        {
            var t = GetPhase(phaseId);
            DeletePhase(t);
        }

        public void DeletePhase(Phase t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phase>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Phase> GetPhases()
        {
            IEnumerable<Phase> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phase>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Phase> GetPhases(bool active)
        {
            IEnumerable<Phase> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phase>();
                t = rep.Find("Where Active=1");
            }
            return t;
        }
        public IEnumerable<Phase> GetPhaseDropDownItems(bool active)
        {
            IEnumerable<Phase> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phase>();
                t = rep.Find("Where Active=1").OrderBy(x=>x.GroupIndex).ThenBy(x=>x.PhaseName);
            }
            return t;
        }
        public Phase GetPhase(int phaseId)
        {
            Phase t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phase>();
                t = rep.GetById(phaseId);
            }
            return t;
        }
        public void UpdatePhase(Phase t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Phase>();
                rep.Update(t);
            }
        }
    }
}
