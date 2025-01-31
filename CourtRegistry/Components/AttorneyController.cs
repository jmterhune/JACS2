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
    internal class AttorneyController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateAttorney(Attorney t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Insert(t);
            }
        }
        public void DeleteAttorney(int attorneyId)
        {
            var t = GetAttorney(attorneyId);
            DeleteAttorney(t);
        }
        public void DeleteAttorney(Attorney t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Attorney> GetAttornies()
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Get();
            }
            return t;
        }
        public Attorney GetAttorney(int attorneyId)
        {
            Attorney t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.GetById(attorneyId);
            }
            return t;
        }
        public void UpdateAttorney(Attorney t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Update(t);
            }
        }
        public IEnumerable<RegistryListItem> GetAttorneyRegistry(int locationId, int year,int caseTypeId, int jacCode)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
              return  ctx.ExecuteQuery<RegistryListItem>(System.Data.CommandType.StoredProcedure, "tjc_car_get_registry_list", locationId,year,caseTypeId,jacCode);
            }
        }
        public IEnumerable<JacCode> GetAttorneyJacCode(int attorneyId,int locationId, int year)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<JacCode>(System.Data.CommandType.StoredProcedure, "tjc_car_get_attorney_jac_codes", attorneyId,locationId, year);
            }
        }
    }
}
