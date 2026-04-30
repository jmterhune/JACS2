using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Helpers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class EeoController
    {
        public EeoInfo GetById(long id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EeoInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<EeoInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EeoInfo>();
                return rep.Get();
            }
        }

        public long Create(EeoInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EeoInfo>();
                rep.Insert(item);
            }
            return item.EeoId;
        }

        public void Update(EeoInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            // Preserve audit columns from the existing row (JSON payloads come
            // in with DateTime.MinValue / 0 which SQL Server datetime rejects).
            var existing = GetById(item.EeoId);
            if (existing != null)
            {
                item.CreatedDate = existing.CreatedDate;
                item.CreatedById = existing.CreatedById;
            }
            else
            {
                item.CreatedDate = DateTime.Now;
                item.CreatedById = userId;
            }
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EeoInfo>();
                rep.Update(item);
            }
        }

        public void Delete(long id)
        {
            var item = GetById(id);
            if (item != null)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<EeoInfo>();
                    rep.Delete(item);
                }
            }
        }

        public IEnumerable<EeoInfo> GetByYear(int year)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EeoInfo>();
                return rep.Find("WHERE [Year] = @0", year);
            }
        }

        // EEO counts are scoped to actual employees only — IsEmployee = 1.
        // Vendors / contractors / terminated user shells in tjc_employee
        // shouldn't show up in compliance reports.

        // Count distinct employees whose position history overlaps the reporting window.
        public int GetGenderCount(int jobGroupId, string gender, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT COUNT(DISTINCT e.EmployeeId)
                               FROM tjc_employee e
                               JOIN tjc_employee_position_history ph ON ph.SocialSecurityNumber = e.SocialSecurityNumber
                               WHERE e.IsEmployee = 1
                                 AND e.JobGroupId = @0
                                 AND e.Gender = @1
                                 AND ph.StartDate <= @3
                                 AND (ph.EndDate IS NULL OR ph.EndDate >= @2)";
                return ctx.ExecuteScalar<int>(CommandType.Text, sql, jobGroupId, gender, startDate, endDate);
            }
        }

        public int GetRaceCount(int jobGroupId, string race, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT COUNT(DISTINCT e.EmployeeId)
                               FROM tjc_employee e
                               JOIN tjc_employee_position_history ph ON ph.SocialSecurityNumber = e.SocialSecurityNumber
                               WHERE e.IsEmployee = 1
                                 AND e.JobGroupId = @0
                                 AND e.Race = @1
                                 AND ph.StartDate <= @3
                                 AND (ph.EndDate IS NULL OR ph.EndDate >= @2)";
                return ctx.ExecuteScalar<int>(CommandType.Text, sql, jobGroupId, race, startDate, endDate);
            }
        }

        public int GetGenderHireCount(int jobGroupId, string gender, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT COUNT(*) FROM tjc_employee
                               WHERE IsEmployee = 1
                                 AND JobGroupId = @0 AND Gender = @1 AND HireDate BETWEEN @2 AND @3";
                return ctx.ExecuteScalar<int>(CommandType.Text, sql, jobGroupId, gender, startDate, endDate);
            }
        }

        public int GetRaceHireCount(int jobGroupId, string race, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT COUNT(*) FROM tjc_employee
                               WHERE IsEmployee = 1
                                 AND JobGroupId = @0 AND Race = @1 AND HireDate BETWEEN @2 AND @3";
                return ctx.ExecuteScalar<int>(CommandType.Text, sql, jobGroupId, race, startDate, endDate);
            }
        }

        public int GetGenderPromoTransferCount(int jobGroupId, string gender, string type, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT COUNT(*) FROM tjc_employee_position_history ph
                               JOIN tjc_employee e ON e.SocialSecurityNumber = ph.SocialSecurityNumber
                               WHERE e.IsEmployee = 1
                                 AND e.JobGroupId = @0 AND e.Gender = @1 AND ph.EntryType = @2
                                 AND ph.StartDate BETWEEN @3 AND @4";
                return ctx.ExecuteScalar<int>(CommandType.Text, sql, jobGroupId, gender, type, startDate, endDate);
            }
        }

        public int GetRacePromoTransferCount(int jobGroupId, string race, string type, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT COUNT(*) FROM tjc_employee_position_history ph
                               JOIN tjc_employee e ON e.SocialSecurityNumber = ph.SocialSecurityNumber
                               WHERE e.IsEmployee = 1
                                 AND e.JobGroupId = @0 AND e.Race = @1 AND ph.EntryType = @2
                                 AND ph.StartDate BETWEEN @3 AND @4";
                return ctx.ExecuteScalar<int>(CommandType.Text, sql, jobGroupId, race, type, startDate, endDate);
            }
        }

        public int GetGenderTerminationCount(int jobGroupId, string gender, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT COUNT(*) FROM tjc_employee
                               WHERE IsEmployee = 1
                                 AND JobGroupId = @0 AND Gender = @1 AND TerminationDate BETWEEN @2 AND @3";
                return ctx.ExecuteScalar<int>(CommandType.Text, sql, jobGroupId, gender, startDate, endDate);
            }
        }

        public int GetRaceTerminationCount(int jobGroupId, string race, DateTime startDate, DateTime endDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT COUNT(*) FROM tjc_employee
                               WHERE IsEmployee = 1
                                 AND JobGroupId = @0 AND Race = @1 AND TerminationDate BETWEEN @2 AND @3";
                return ctx.ExecuteScalar<int>(CommandType.Text, sql, jobGroupId, race, startDate, endDate);
            }
        }

        // Iterates all job groups, computes the full stat set for each, and inserts (or updates) the EeoInfo row for the year.
        public void SaveYearStats(int year, DateTime startDate, DateTime endDate, int userId = -1)
        {
            var jobGroupController = new JobGroupController();
            var jobGroups = jobGroupController.GetAll();

            foreach (var jg in jobGroups)
            {
                var item = new EeoInfo
                {
                    JobGroupId = jg.JobGroupId,
                    Year = year,

                    PopulationMale = GetGenderCount(jg.JobGroupId, "Male", startDate, endDate),
                    PopulationFemale = GetGenderCount(jg.JobGroupId, "Female", startDate, endDate),
                    PopulationWhite = GetRaceCount(jg.JobGroupId, "White", startDate, endDate),
                    PopulationBlack = GetRaceCount(jg.JobGroupId, "Black", startDate, endDate),
                    PopulationAsian = GetRaceCount(jg.JobGroupId, "Asian", startDate, endDate),
                    PopulationIndian = GetRaceCount(jg.JobGroupId, "Indian", startDate, endDate),
                    PopulationHispanic = GetRaceCount(jg.JobGroupId, "Hispanic", startDate, endDate),
                    PopulationOther = GetRaceCount(jg.JobGroupId, "Other", startDate, endDate),

                    HireMale = GetGenderHireCount(jg.JobGroupId, "Male", startDate, endDate),
                    HireFemale = GetGenderHireCount(jg.JobGroupId, "Female", startDate, endDate),
                    HireWhite = GetRaceHireCount(jg.JobGroupId, "White", startDate, endDate),
                    HireBlack = GetRaceHireCount(jg.JobGroupId, "Black", startDate, endDate),
                    HireAsian = GetRaceHireCount(jg.JobGroupId, "Asian", startDate, endDate),
                    HireIndian = GetRaceHireCount(jg.JobGroupId, "Indian", startDate, endDate),
                    HireHispanic = GetRaceHireCount(jg.JobGroupId, "Hispanic", startDate, endDate),
                    HireOther = GetRaceHireCount(jg.JobGroupId, "Other", startDate, endDate),

                    PromoMale = GetGenderPromoTransferCount(jg.JobGroupId, "Male", "Promotion", startDate, endDate),
                    PromoFemale = GetGenderPromoTransferCount(jg.JobGroupId, "Female", "Promotion", startDate, endDate),
                    PromoWhite = GetRacePromoTransferCount(jg.JobGroupId, "White", "Promotion", startDate, endDate),
                    PromoBlack = GetRacePromoTransferCount(jg.JobGroupId, "Black", "Promotion", startDate, endDate),
                    PromoAsian = GetRacePromoTransferCount(jg.JobGroupId, "Asian", "Promotion", startDate, endDate),
                    PromoIndian = GetRacePromoTransferCount(jg.JobGroupId, "Indian", "Promotion", startDate, endDate),
                    PromoHispanic = GetRacePromoTransferCount(jg.JobGroupId, "Hispanic", "Promotion", startDate, endDate),
                    PromoOther = GetRacePromoTransferCount(jg.JobGroupId, "Other", "Promotion", startDate, endDate),

                    TransferMale = GetGenderPromoTransferCount(jg.JobGroupId, "Male", "Transfer", startDate, endDate),
                    TransferFemale = GetGenderPromoTransferCount(jg.JobGroupId, "Female", "Transfer", startDate, endDate),
                    TransferWhite = GetRacePromoTransferCount(jg.JobGroupId, "White", "Transfer", startDate, endDate),
                    TransferBlack = GetRacePromoTransferCount(jg.JobGroupId, "Black", "Transfer", startDate, endDate),
                    TransferAsian = GetRacePromoTransferCount(jg.JobGroupId, "Asian", "Transfer", startDate, endDate),
                    TransferIndian = GetRacePromoTransferCount(jg.JobGroupId, "Indian", "Transfer", startDate, endDate),
                    TransferHispanic = GetRacePromoTransferCount(jg.JobGroupId, "Hispanic", "Transfer", startDate, endDate),
                    TransferOther = GetRacePromoTransferCount(jg.JobGroupId, "Other", "Transfer", startDate, endDate),

                    TermMale = GetGenderTerminationCount(jg.JobGroupId, "Male", startDate, endDate),
                    TermFemale = GetGenderTerminationCount(jg.JobGroupId, "Female", startDate, endDate),
                    TermWhite = GetRaceTerminationCount(jg.JobGroupId, "White", startDate, endDate),
                    TermBlack = GetRaceTerminationCount(jg.JobGroupId, "Black", startDate, endDate),
                    TermAsian = GetRaceTerminationCount(jg.JobGroupId, "Asian", startDate, endDate),
                    TermIndian = GetRaceTerminationCount(jg.JobGroupId, "Indian", startDate, endDate),
                    TermHispanic = GetRaceTerminationCount(jg.JobGroupId, "Hispanic", startDate, endDate),
                    TermOther = GetRaceTerminationCount(jg.JobGroupId, "Other", startDate, endDate)
                };

                EeoInfo existing;
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<EeoInfo>();
                    existing = rep.Find("WHERE [Year] = @0 AND JobGroupId = @1", year, jg.JobGroupId).FirstOrDefault();
                }

                if (existing == null)
                {
                    Create(item, userId);
                }
                else
                {
                    item.EeoId = existing.EeoId;
                    item.CreatedDate = existing.CreatedDate;
                    item.CreatedById = existing.CreatedById;
                    Update(item, userId);
                }
            }
        }
    }
}
