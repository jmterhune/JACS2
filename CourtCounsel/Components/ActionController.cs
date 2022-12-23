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
    internal class ActionController
    {
        private const string CONN_INTRANET = "Intranet.API"; //Connection
        public void CreateAction(Action t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Action>();
                rep.Insert(t);
            }
        }

        public void DeleteAction(int actionId)
        {
            var t = GetAction(actionId);
            DeleteAction(t);
        }

        public void DeleteAction(Action t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Action>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Action> GetActions()
        {
            IEnumerable<Action> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Action>();
                t = rep.Get();
            }
            return t;
        }
        public bool ActionExists(int actionId)
        {
            Action t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Action>();
                t = rep.GetById(actionId);
            }
            return t.ActionId > 0;
        }
        public IEnumerable<Action> GetActiveActions()
        {
            IEnumerable<Action> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Action>();
                t = rep.Find("Where Active=1");
            }
            return t;
        }

        public Action GetAction(int actionId)
        {
            Action t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Action>();
                t = rep.GetById(actionId);
            }
            return t;
        }
        public void UpdateAction(Action t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Action>();
                rep.Update(t);
            }
        }
    }
}
