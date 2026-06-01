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
using DotNetNuke.Services.Mail;
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace tjc.Modules.CourtRegistry.Components
{
    internal class AttorneyController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
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
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Attorney> GetAttornies()
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Get();
            }
            return t;
        }
        public Attorney GetAttorney(int attorneyId)
        {
            Attorney t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.GetById(attorneyId);
            }
            return t;
        }
        public IEnumerable<Attorney> GetAttorneys(bool showAll, int year)
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                if (showAll)
                    t = rep.Find("Where Email IS NOT NULL AND Email <>''");
                else
                    t = GetAttorneysByApplicationYear(year);
            }
            return t;
        }
        public IEnumerable<Attorney> GetAttorneysByApplicationYear(int year)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<Attorney>(System.Data.CommandType.Text,
                    @"SELECT DISTINCT atty.*
                      FROM tjc_car_attorneys atty
                      JOIN tjc_car_applications a ON atty.AttorneyId = a.AttorneyId
                      WHERE a.[Year] = @0
                        AND atty.Email IS NOT NULL AND atty.Email <> ''
                      ORDER BY atty.LastName, atty.FirstName", year);
            }
        }
        public void UpdateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Update(t);
            }
        }
        public IEnumerable<RegistryListItem> GetAttorneyRegistry(int locationId, int year, int caseTypeId, int jacCode)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<RegistryListItem>(System.Data.CommandType.Text,
                    @"SELECT DISTINCT atty.*
                      FROM tjc_car_attorneys atty
                      JOIN tjc_car_applications a ON atty.AttorneyId = a.AttorneyId
                      JOIN tjc_car_application_by_jac_code ajac ON a.ApplicationId = ajac.ApplicationId
                      JOIN tjc_car_jac_codes jc ON ajac.JacCodeId = jc.JacCodeId
                      WHERE (@0 <= 0 OR ajac.LocationId = @0)
                        AND a.[Year] = @1
                        AND (ajac.Status = 1 OR ajac.Status = 4)
                        AND (@2 <= 0 OR jc.CaseTypeId = @2)
                        AND (@3 <= 0 OR ajac.JacCodeId = @3)
                      ORDER BY atty.LastName, atty.FirstName, atty.LawFirm",
                    locationId, year, caseTypeId, jacCode);
            }
        }
        public IEnumerable<JacCode> GetAttorneyJacCode(int attorneyId, int locationId, int year)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<JacCode>(System.Data.CommandType.Text,
                    @"SELECT DISTINCT jc.*
                      FROM tjc_car_attorneys atty
                      JOIN tjc_car_applications a ON atty.AttorneyId = a.AttorneyId
                      JOIN tjc_car_application_by_jac_code ajac ON a.ApplicationId = ajac.ApplicationId
                      JOIN tjc_car_jac_codes jc ON ajac.JacCodeId = jc.JacCodeId
                      WHERE atty.AttorneyId = @0
                        AND (@1 <= 0 OR ajac.LocationId = @1)
                        AND a.[Year] = @2
                        AND (ajac.Status = 1 OR ajac.Status = 4)
                      ORDER BY jc.JacCodeId",
                    attorneyId, locationId, year);
            }
        }

        private static readonly string[] _attySortColumns =
        {
            "AttorneyID","BarNumber","LastName","FirstName","Email","Phone","Cell","Fax","LawFirm"
        };

        internal int GetAttorneyListCount(int barNumber, string firstName, string lastName, string email, string lawFirm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteScalar<int>(System.Data.CommandType.Text,
                    @"SELECT COUNT(*)
                      FROM tjc_car_attorneys
                      WHERE (@0 <= 0 OR BarNumber = @0)
                        AND (@1 IS NULL OR @1 = '' OR FirstName LIKE @1 + '%')
                        AND (@2 IS NULL OR @2 = '' OR LastName  LIKE @2 + '%')
                        AND (@3 IS NULL OR @3 = '' OR Email     LIKE '%' + @3 + '%')
                        AND (@4 IS NULL OR @4 = '' OR LawFirm   LIKE '%' + @4 + '%')",
                    barNumber, firstName, lastName, email, lawFirm);
            }
        }

        internal IEnumerable<Attorney> GetAttorneyListPaged(int barNumber, string firstName, string lastName, string email, string lawFirm, int recordOffset, int pageSize, string sortColumn, string sortDirection)
        {
            string sortCol = _attySortColumns.FirstOrDefault(c => c.Equals(sortColumn, StringComparison.OrdinalIgnoreCase)) ?? "AttorneyID";
            string sortDir = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            string sql = $@"SELECT *
                            FROM tjc_car_attorneys
                            WHERE (@0 <= 0 OR BarNumber = @0)
                              AND (@1 IS NULL OR @1 = '' OR FirstName LIKE @1 + '%')
                              AND (@2 IS NULL OR @2 = '' OR LastName  LIKE @2 + '%')
                              AND (@3 IS NULL OR @3 = '' OR Email     LIKE '%' + @3 + '%')
                              AND (@4 IS NULL OR @4 = '' OR LawFirm   LIKE '%' + @4 + '%')
                            ORDER BY {sortCol} {sortDir}
                            OFFSET @5 ROWS FETCH NEXT @6 ROWS ONLY";

            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<Attorney>(System.Data.CommandType.Text, sql,
                    barNumber, firstName, lastName, email, lawFirm, recordOffset, pageSize);
            }
        }
    }
}
