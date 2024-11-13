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
    internal class DefendantController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateDefendant(Defendant t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Defendant>();
                rep.Insert(t);
            }
        }

        public void DeleteDefendant(int defendantId)
        {
            var t = GetDefendant(defendantId);
            DeleteDefendant(t);
        }

        public void DeleteDefendant(Defendant t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Defendant>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Defendant> GetDefendantes()
        {
            IEnumerable<Defendant> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Defendant>();
                t = rep.Get();
            }
            return t;
        }
        public bool DefendantExists(int defendantId)
        {
            Defendant t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Defendant>();
                t = rep.GetById(defendantId);
            }
            return t.DefendantID > 0;
        }
        public Defendant GetDefendant(int defendantId)
        {
            Defendant t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Defendant>();
                t = rep.GetById(defendantId);
            }
            return t;
        }
        public void UpdateDefendant(Defendant t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Defendant>();
                rep.Update(t);
            }
        }
    }
}
