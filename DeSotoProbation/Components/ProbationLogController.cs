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

namespace tjc.Modules.DeSoto.Probation.Components
{
    internal class ProbationLogController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateProbationLog(ProbationLog t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationLog>();
                rep.Insert(t);
            }
        }

        public void DeleteProbationLog(int phoneId)
        {
            var t = GetProbationLog(phoneId);
            DeleteProbationLog(t);
        }

        public void DeleteProbationLog(ProbationLog t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationLog>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ProbationLog> GetProbationLoges()
        {
            IEnumerable<ProbationLog> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationLog>();
                t = rep.Get();
            }
            return t;
        }
        public bool ProbationLogExists(int phoneId)
        {
            ProbationLog t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationLog>();
                t = rep.GetById(phoneId);
            }
            return t.RecordID > 0;
        }
        public ProbationLog GetProbationLog(int phoneId)
        {
            ProbationLog t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationLog>();
                t = rep.GetById(phoneId);
            }
            return t;
        }
        public void UpdateProbationLog(ProbationLog t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationLog>();
                rep.Update(t);
            }
        }
    }
}
