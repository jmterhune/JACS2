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
    internal class ExcludedAttorneysController
    {
        private const string CONN_INTRANET = "jacsDesoto";

        public IEnumerable<ExcludedAttorney> GetAttorneys()
        {
            IEnumerable<ExcludedAttorney> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<ExcludedAttorney>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<ExcludedAttorneyView> GetAttorneyView()
        {
            IEnumerable<ExcludedAttorneyView> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<ExcludedAttorneyView>();
                t = rep.Get();
            }
            return t;
        }
        public void CreateAttorney(ExcludedAttorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<ExcludedAttorney>();
                rep.Insert(t);
            }
        }
        public ExcludedAttorney GetAttorney(int recordId)
        {
            ExcludedAttorney t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<ExcludedAttorney>();
                t = rep.GetById(recordId);
            }
            return t;
        }
        public void DeleteAttorney(int recordId)
        {
            var t = GetAttorney(recordId);
            DeleteAttorney(t);
        }

        public void DeleteAttorney(ExcludedAttorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<ExcludedAttorney>();
                rep.Delete(t);
            }
        }
    }
}
