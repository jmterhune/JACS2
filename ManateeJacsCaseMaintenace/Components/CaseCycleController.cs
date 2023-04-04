/*
' Copyright (c) 2023 jterhune
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
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.JacsCaseMaint.Components
{
    internal class CaseCycleController
    {

        private const string CONN_INTRANET = "jacsManatee";

        public IEnumerable<CaseCycle> GetCaseCyles(string year,string caseType, string sequence)
        {
            IEnumerable<CaseCycle> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                string casenumber = string.Format("%{0}%{1}%{2}%", year, caseType, sequence);
                var rep = ctx.GetRepository<CaseCycle>();
                t = rep.Find("Where CaseNum like @0", casenumber);
            }
            return t;
        }

        public CaseCycle GetCaseCycleByCaseId(int caseId)
        {
            CaseCycle t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<CaseCycle>();
                t = rep.Find("Where FLRC_Id=@0",caseId).FirstOrDefault();
            }
            return t;
        }
        public CaseCycle GetCaseCycle(int caseCycleId)
        {
            CaseCycle t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<CaseCycle>();
                t = rep.GetById(caseCycleId);
            }
            return t;
        }
        public void DeleteCaseCycle(int caseCycleId)
        {
            var t = GetCaseCycle(caseCycleId);
            DeleteCaseCycle(t);
        }

        public void DeleteCaseCycle(CaseCycle t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<CaseCycle>();
                rep.Delete(t);
            }
        }
    }
}
