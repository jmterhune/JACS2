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
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace tjc.Modules.CourtRegistry.Components
{
    internal class JacCodeController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateJacCode(JacCode t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
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
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                rep.Delete(t);
            }
        }
        public IEnumerable<JacCode> GetJacCodes()
        {
            IEnumerable<JacCode> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<JacCode> GetJacCodesByCaseType(int caseTypeId)
        {
            IEnumerable<JacCode> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                t = rep.Find("Where CaseTypeID=@0", caseTypeId);
            }
            return t;
        }
        public JacCode GetJacCode(int jacCodeId)
        {
            JacCode t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                t = rep.GetById(jacCodeId);
            }
            return t;
        }

        public void UpdateJacCode(JacCode t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCode>();
                rep.Update(t);
            }
        }
        //
        public void CreateJacCodeUpdate(JacCodeUpdate t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
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
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                rep.Delete(t);
            }
        }

        public IEnumerable<JacCodeUpdate> GetJacCodeUpdates()
        {
            IEnumerable<JacCodeUpdate> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                t = rep.Get();
            }
            return t;
        }

        public JacCodeUpdate GetJacCodeUpdate(int jacCodeId)
        {
            JacCodeUpdate t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                t = rep.GetById(jacCodeId);
            }
            return t;
        }

        public void UpdateJacCodeUpdate(JacCodeUpdate t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeUpdate>();
                rep.Update(t);
            }
        }

        internal void ClearExceptions(int appYear)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text, "Delete from tjc_car_jac_code_config Where Year = @0", appYear);
            }
        }

        public void DeleteException(int jacCodeId, int locationId, int year)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text, "Delete from tjc_car_jac_code_config Where JacCodeID = @0 AND LocationID = @1 AND Year = @2", jacCodeId, locationId, year);
            }
        }

        public void DeleteJacCodeConfig(JacCodeConfig t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeConfig>();
                rep.Delete(t);
            }
        }
        public IEnumerable<JacException> GetJacExceptions(int year)
        {
            IEnumerable<JacException> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacException>();
                t = rep.Find("Where Year=@0", year);
            }
            return t;
        }

        public JacCodeConfig GetJacCodeConfig(int jacCodeId, int locationId, int year)
        {
            JacCodeConfig t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<JacCodeConfig>();
                t = rep.Find("Where JacCodeID=@0 AND LocationID = @1 AND Year = @2", jacCodeId, locationId, year).FirstOrDefault();
            }
            return t;
        }
        internal void CreateJacCodeConfig(JacCodeConfig config)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_car_add_exclusion", config.JacCodeID, config.LocationID, config.Year, config.Exclude, config.OnlyRenewals);
            }
        }
        internal void UpdateJacCode(JacCodeConfig config)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_car_update_exclusion", config.JacCodeID, config.LocationID, config.Year, config.Exclude, config.OnlyRenewals);
            }
        }

    }
}
