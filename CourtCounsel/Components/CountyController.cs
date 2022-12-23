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
    internal class CountyController
    {
        private const string CONN_INTRANET = "Intranet.API"; //Connection
        public void CreateCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<County>();
                rep.Insert(t);
            }
        }

        public void DeleteCounty(int countyId)
        {
            var t = GetCounty(countyId);
            DeleteCounty(t);
        }

        public void DeleteCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<County>();
                rep.Delete(t);
            }
        }

        public IEnumerable<County> GetCounties()
        {
            IEnumerable<County> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.Get();
            }
            return t;
        }
        public bool CountyExists(int countyId)
        {
            County t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.GetById(countyId);
            }
            return t.CountyId > 0;
        }
 
        public County GetCounty(int countyId)
        {
            County t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.GetById(countyId);
            }
            return t;
        }
        public void UpdateCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<County>();
                rep.Update(t);
            }
        }
    }
}
