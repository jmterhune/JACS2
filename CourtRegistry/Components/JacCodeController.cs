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
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<JacException>(System.Data.CommandType.Text,
                    @"SELECT cfg.JacCodeId AS JacCodeID, cfg.LocationId AS LocationID, cfg.[Year],
                             cfg.Exclude, cfg.OnlyRenewals, l.LocationName
                      FROM tjc_car_jac_code_config cfg
                      JOIN tjc_car_locations l ON cfg.LocationId = l.LocationId
                      WHERE cfg.[Year] = @0
                      ORDER BY cfg.JacCodeId, l.LocationName", year);
            }
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
                ctx.Execute(System.Data.CommandType.Text,
                    @"INSERT INTO tjc_car_jac_code_config (JacCodeId, LocationId, [Year], Exclude, OnlyRenewals)
                      VALUES (@0, @1, @2, @3, @4)",
                    config.JacCodeID, config.LocationID, config.Year, config.Exclude, config.OnlyRenewals);
            }
        }
        internal void UpdateJacCode(JacCodeConfig config)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    @"UPDATE tjc_car_jac_code_config
                      SET Exclude = @3, OnlyRenewals = @4
                      WHERE JacCodeId = @0 AND LocationId = @1 AND [Year] = @2",
                    config.JacCodeID, config.LocationID, config.Year, config.Exclude, config.OnlyRenewals);
            }
        }

    }
}
