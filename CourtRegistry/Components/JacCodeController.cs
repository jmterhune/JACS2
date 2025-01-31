/*
' Copyright (c) 2025 Joe Terhune
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

namespace tjc.Modules.CourtRegistry.Components
{
    internal class JacCodeController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateJacCode(JacCode t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                rep.Insert(t);
            }
        }

        public void DeleteJacCode(int jacCodeId)
        {
            var t = GetJacCode(jacCodeId);
            DeleteJacCode(t);
        }

        public void DeleteJacCode(JacCode t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                rep.Delete(t);
            }
        }
        public IEnumerable<JacCode> GetJacCodes()
        {
            IEnumerable<JacCode> t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<JacCode> GetJacCodesByCaseType(int caseTypeId)
        {
            IEnumerable<JacCode> t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                t = rep.Find("Where CaseTypeID=@0",caseTypeId);
            }
            return t;
        }
        public JacCode GetJacCode(int jacCodeId)
        {
            JacCode t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                t = rep.GetById(jacCodeId);
            }
            return t;
        }

        public void UpdateJacCode(JacCode t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                rep.Update(t);
            }
        }
        //
        public void CreateJacCodeUpdate(JacCodeUpdate t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                rep.Insert(t);
            }
        }

        public void DeleteJacCodeUpdate(int jacCodeId)
        {
            var t = GetJacCodeUpdate(jacCodeId);
            DeleteJacCodeUpdate(t);
        }

        public void DeleteJacCodeUpdate(JacCodeUpdate t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                rep.Delete(t);
            }
        }

        public IEnumerable<JacCodeUpdate> GetJacCodeUpdates()
        {
            IEnumerable<JacCodeUpdate> t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                t = rep.Get();
            }
            return t;
        }

        public JacCodeUpdate GetJacCodeUpdate(int jacCodeId)
        {
            JacCodeUpdate t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                t = rep.GetById(jacCodeId);
            }
            return t;
        }

        public void UpdateJacCodeUpdate(JacCodeUpdate t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                rep.Update(t);
            }
        }
    }
}
