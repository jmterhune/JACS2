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
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace tjc.Modules.CourtRegistry.Components
{
    internal class ApplicationController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateApplication(Application t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                rep.Insert(t);
            }
        }

        public void DeleteApplication(int applicationId)
        {
            var t = GetApplication(applicationId);
            DeleteApplication(t);
        }

        public void DeleteApplication(Application t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Application> GetApplications()
        {
            IEnumerable<Application> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                t = rep.Get();
            }
            return t;
        }

        public Application GetApplication(int applicationId)
        {
            Application t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                t = rep.GetById(applicationId);
            }
            return t;
        }

        public void UpdateApplication(Application t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                rep.Update(t);
            }
        }
        //Application Periods
        public void CreateApplicationPeriod(ApplicationPeriod t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                rep.Insert(t);
            }
        }

        public void DeleteApplicationPeriod(int applicationYear)
        {
            var t = GetApplicationPeriod(applicationYear);
            DeleteApplicationPeriod(t);
        }

        public void DeleteApplicationPeriod(ApplicationPeriod t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ApplicationPeriod> GetApplicationPeriods()
        {
            IEnumerable<ApplicationPeriod> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                t = rep.Get();
            }
            return t;
        }

        public ApplicationPeriod GetApplicationPeriod(int applicationYear)
        {
            ApplicationPeriod t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                t = rep.GetById(applicationYear);
            }
            return t;
        }

        public void UpdateApplicationPeriod(ApplicationPeriod t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                rep.Update(t);
            }
        }

        private static readonly string[] _appSortColumns =
        {
            "ApplicationID","Year","LastName","FirstName","DateCreated",
            "DateReviewed","YearsOnRegistry","IsRenewal","GuardianSignature","Status"
        };

        internal int GetApplicationListCount(int applicationId, int periodYear, string firstName, string lastName, int statusId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteScalar<int>(System.Data.CommandType.Text,
                    @"SELECT COUNT(*)
                      FROM tjc_car_applications a
                      JOIN tjc_car_attorneys atty ON a.AttorneyId = atty.AttorneyId
                      WHERE (@0 <= 0 OR a.ApplicationId = @0)
                        AND (@1 <= 0 OR a.[Year] = @1)
                        AND (@4 < 0 OR ISNULL(a.Status, 0) = @4)
                        AND (@2 IS NULL OR @2 = '' OR atty.FirstName LIKE @2 + '%')
                        AND (@3 IS NULL OR @3 = '' OR atty.LastName  LIKE @3 + '%')",
                    applicationId, periodYear, firstName, lastName, statusId);
            }
        }

        internal IEnumerable<ApplicationListItem> GetApplicationListPaged(int applicationId, int periodYear, string firstName, string lastName, int statusId, int recordOffset, int pageSize, string sortColumn, string sortDirection)
        {
            string sortCol = _appSortColumns.FirstOrDefault(c => c.Equals(sortColumn, StringComparison.OrdinalIgnoreCase)) ?? "ApplicationID";
            string sortDir = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            string sql = $@"SELECT a.ApplicationId AS ApplicationID,
                                  ISNULL(a.AttorneyId, 0) AS AttorneyID,
                                  atty.LastName, atty.FirstName,
                                  a.GuardianSignature,
                                  ISNULL(a.Status, 0) AS Status,
                                  ISNULL(a.[Year], 0) AS [Year],
                                  ISNULL(a.YearsOnRegistry, 0) AS YearsOnRegistry,
                                  ISNULL(a.DateCreated, '19000101') AS DateCreated,
                                  a.DateReviewed,
                                  ISNULL(a.IsRenewal, 0) AS IsRenewal,
                                  CAST(CASE WHEN a.GuardianSignature IS NULL OR a.GuardianSignature = '' THEN 0 ELSE 1 END AS BIT) AS IsGuardian
                           FROM tjc_car_applications a
                           JOIN tjc_car_attorneys atty ON a.AttorneyId = atty.AttorneyId
                           WHERE (@0 <= 0 OR a.ApplicationId = @0)
                             AND (@1 <= 0 OR a.[Year] = @1)
                             AND (@4 < 0 OR ISNULL(a.Status, 0) = @4)
                             AND (@2 IS NULL OR @2 = '' OR atty.FirstName LIKE @2 + '%')
                             AND (@3 IS NULL OR @3 = '' OR atty.LastName  LIKE @3 + '%')
                           ORDER BY {sortCol} {sortDir}
                           OFFSET @5 ROWS FETCH NEXT @6 ROWS ONLY";

            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<ApplicationListItem>(System.Data.CommandType.Text, sql,
                    applicationId, periodYear, firstName, lastName, statusId, recordOffset, pageSize);
            }
        }

        public IEnumerable<ApplicationJacCodeDetail> GetApplicationJacCodes(int applicationId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<ApplicationJacCodeDetail>(System.Data.CommandType.Text,
                    @"SELECT ajc.JacCodeID, ajc.LocationID, ajc.ApplicationID, ajc.Status,
                             jc.Category, ct.CaseTypeID, ct.CaseTypeName, l.LocationName
                      FROM tjc_car_application_by_jac_code ajc
                      JOIN tjc_car_jac_codes jc ON ajc.JacCodeID = jc.JacCodeID
                      LEFT JOIN tjc_car_case_types ct ON jc.CaseTypeID = ct.CaseTypeID
                      JOIN tjc_car_locations l ON ajc.LocationID = l.LocationID
                      WHERE ajc.ApplicationID = @0
                      ORDER BY ct.CaseTypeName, jc.Category, jc.JacCodeID, l.LocationName", applicationId);
            }
        }

        public ApplicationJacCode GetApplicationJacCode(int jacCodeId, int locationId, int applicationId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationJacCode>();
                return rep.Find("WHERE JacCodeID = @0 AND LocationID = @1 AND ApplicationID = @2", jacCodeId, locationId, applicationId).FirstOrDefault();
            }
        }

        public void UpdateApplicationJacCode(ApplicationJacCode item)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "UPDATE tjc_car_application_by_jac_code SET Status = @0 WHERE JacCodeID = @1 AND LocationID = @2 AND ApplicationID = @3",
                    item.Status, item.JacCodeID, item.LocationID, item.ApplicationID);
            }
        }

        public void DeleteApplicationJacCode(ApplicationJacCode item)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "DELETE FROM tjc_car_application_by_jac_code WHERE JacCodeID = @0 AND LocationID = @1 AND ApplicationID = @2",
                    item.JacCodeID, item.LocationID, item.ApplicationID);
            }
        }

        public IEnumerable<ApplicationJacCode> GetApplicationJacCodesRaw(int applicationId, int year)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<ApplicationJacCode>(System.Data.CommandType.Text,
                    @"SELECT ajc.JacCodeID, ajc.LocationID, ajc.ApplicationID, ajc.Status
                      FROM tjc_car_application_by_jac_code ajc
                      JOIN tjc_car_applications a ON ajc.ApplicationID = a.ApplicationID
                      WHERE a.Year = @0", year);
            }
        }

        public IEnumerable<JacCodeCount> GetJacCodeCounts(int year)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<JacCodeCount>(System.Data.CommandType.Text,
                    @"SELECT ct.CaseTypeName, jc.JacCodeID, jc.Category, l.LocationName, ajc.Status, COUNT(*) AS Cnt
                      FROM tjc_car_application_by_jac_code ajc
                      JOIN tjc_car_applications a ON ajc.ApplicationID = a.ApplicationID
                      JOIN tjc_car_jac_codes jc ON ajc.JacCodeID = jc.JacCodeID
                      LEFT JOIN tjc_car_case_types ct ON jc.CaseTypeID = ct.CaseTypeID
                      JOIN tjc_car_locations l ON ajc.LocationID = l.LocationID
                      WHERE a.Year = @0
                      GROUP BY ct.CaseTypeName, jc.JacCodeID, jc.Category, l.LocationName, ajc.Status
                      ORDER BY ct.CaseTypeName, jc.Category, jc.JacCodeID, l.LocationName", year);
            }
        }

        public IEnumerable<JacCodeYearLocation> GetJacCodesByYear(int year, int attorneyId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                if (attorneyId > 0)
                {
                    return ctx.ExecuteQuery<JacCodeYearLocation>(System.Data.CommandType.Text,
                        @"SELECT DISTINCT ajc.JacCodeID, l.LocationName
                          FROM tjc_car_application_by_jac_code ajc
                          JOIN tjc_car_applications a ON ajc.ApplicationID = a.ApplicationID
                          JOIN tjc_car_locations l ON ajc.LocationID = l.LocationID
                          WHERE a.Year = @0 AND a.AttorneyID = @1
                          ORDER BY ajc.JacCodeID", year, attorneyId);
                }
                return ctx.ExecuteQuery<JacCodeYearLocation>(System.Data.CommandType.Text,
                    @"SELECT DISTINCT ajc.JacCodeID, l.LocationName
                      FROM tjc_car_application_by_jac_code ajc
                      JOIN tjc_car_applications a ON ajc.ApplicationID = a.ApplicationID
                      JOIN tjc_car_locations l ON ajc.LocationID = l.LocationID
                      WHERE a.Year = @0
                      ORDER BY ajc.JacCodeID", year);
            }
        }

        public int GetMaxApplicationYear()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT ISNULL(MAX([Year]), 0) FROM tjc_car_applications");
            }
        }

        public IEnumerable<JacExportRow> GetJacExport(int year)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                return ctx.ExecuteQuery<JacExportRow>(System.Data.CommandType.Text,
                    @"SELECT DISTINCT a.AttorneyID, atty.BarNumber, atty.FirstName, atty.LastName, atty.Email,
                             jc.JacCodeID, jc.Category, l.CountyNumber, a.ApplicationID, a.GuardianSignature, a.DateReviewed
                      FROM tjc_car_application_by_jac_code ajc
                      JOIN tjc_car_applications a ON ajc.ApplicationID = a.ApplicationID
                      JOIN tjc_car_attorneys atty ON a.AttorneyID = atty.AttorneyID
                      JOIN tjc_car_jac_codes jc ON ajc.JacCodeID = jc.JacCodeID
                      JOIN tjc_car_locations l ON ajc.LocationID = l.LocationID
                      WHERE a.[Year] = @0
                        AND (ajc.Status = 1 OR ajc.Status = 4)
                        AND a.Exported = 0
                        AND a.Status = 1
                      ORDER BY atty.BarNumber", year);
            }
        }

        public void MarkApplicationExported(int applicationId, DateTime exportDate)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "UPDATE tjc_car_applications SET Exported = 1, ExportDate = @0 WHERE ApplicationID = @1",
                    exportDate, applicationId);
            }
        }

        public void UndoExport(int year)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    @"UPDATE tjc_car_applications SET Exported = 0
                      WHERE [Year] = @0 AND ExportDate IS NOT NULL
                        AND CAST(ExportDate AS DATE) = (SELECT CAST(MAX(ExportDate) AS DATE) FROM tjc_car_applications WHERE [Year] = @0)", year);
            }
        }
    }
    internal class JacCodeCount
    {
        public string CaseTypeName { get; set; }
        public int JacCodeID { get; set; }
        public string Category { get; set; }
        public string LocationName { get; set; }
        public int Status { get; set; }
        public int Cnt { get; set; }
    }
    internal class JacCodeYearLocation
    {
        public int JacCodeID { get; set; }
        public string LocationName { get; set; }
    }
    internal class JacExportRow
    {
        public int AttorneyID { get; set; }
        public int BarNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int JacCodeID { get; set; }
        public string Category { get; set; }
        public int CountyNumber { get; set; }
        public int ApplicationID { get; set; }
        public string GuardianSignature { get; set; }
        public DateTime? DateReviewed { get; set; }
    }
}
