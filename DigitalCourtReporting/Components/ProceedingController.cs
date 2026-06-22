using DotNetNuke.Data;
using System;
using System.Collections.Generic;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    internal class ProceedingController
    {
        public void CreateProceeding(Proceeding t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                int userId = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo().UserID;
                t.CreatedByID = userId > 0 ? userId : 1;
                t.CreatedDate = DateTime.Now;
                t.LastModifiedByID = userId > 0 ? userId : 1;
                t.LastModifiedDate = DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteProceeding(int proceedingId)
        {
            var t = GetProceeding(proceedingId);
            DeleteProceeding(t);
        }
        public void DeleteProceeding(Proceeding t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Proceeding> GetProceedings()
        {
            IEnumerable<Proceeding> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                t = rep.Get();
            }
            return t;
        }

        public IEnumerable<ProceedingListItem> GetProceedingsFiltered(ListTypes listType, SearchTypes searchType, string searchText, int countyId)
        {
            IEnumerable<ProceedingListItem> t;
            string query = string.Empty;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ProceedingListItem>();
                switch (listType)
                {
                    case ListTypes.payment:
                        query = "(Paid = 0 OR Paid IS NULL)";
                        break;
                    case ListTypes.notification:
                        query = "Paid = 1 AND (Closed = 0 OR Closed IS NULL) AND CA = 1";
                        break;
                    case ListTypes.cdCreation:
                        query = "Paid = 1 AND (Closed = 0 OR Closed IS NULL) AND (CA = 0 OR CA IS NULL)";
                        break;
                    case ListTypes.completed:
                        query = "Paid = 1 AND Closed = 1 AND CA = 1";
                        break;
                    case ListTypes.inquiry:
                        query = "(Closed = 0 OR Closed IS NULL) AND IsInquiry = 1";
                        break;
                    default:
                        break;

                }
                if (countyId > 0)
                {
                    if (!string.IsNullOrEmpty(query))
                        query += string.Format(" AND JurisdictionID ={0}", countyId);
                    else
                        query += string.Format("JurisdictionID ={0}", countyId);
                }
                if (!string.IsNullOrEmpty(searchText))
                {
                    switch (searchType)
                    {
                        case SearchTypes.caseName:
                            if (!string.IsNullOrEmpty(query))
                                query += " AND ";
                            query += string.Format("LOWER(CaseName) LIKE '%{0}%'", searchText.ToLower());
                            break;
                        case SearchTypes.caseNumber:
                            if (!string.IsNullOrEmpty(query))
                                query += " AND ";
                            query += string.Format("LOWER(CaseNumber) LIKE '%{0}%'", searchText.ToLower());
                            break;
                        case SearchTypes.trackingNumber:
                            if (!string.IsNullOrEmpty(query))
                                query += " AND ";
                            query += string.Format("EXISTS(Select ProcessingID FROM tjc_dcr_audio Where ProcessingID=tjc_dcr_proceeding.ProcessingID AND LOWER(Tracking) LIKE  +''%''+ @searchText + ''%'')", searchText.ToLower());
                            break;
                        case SearchTypes.requestor:
                            if (!string.IsNullOrEmpty(query))
                                query += " AND ";
                            query += string.Format("LOWER(Requestor) LIKE '%{0}%'", searchText.ToLower());
                            break;
                        default:
                            break;
                    }
                }
                query = "Where " + query;
                t = rep.Find(query);
            }
            return t;
        }

        public Proceeding GetProceeding(int proceedingId)
        {
            Proceeding t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                t = rep.GetById(proceedingId);
            }
            return t;
        }
        public ProceedingListItem GetProceedingListItem(int proceedingId)
        {
            ProceedingListItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ProceedingListItem>();
                t = rep.GetById(proceedingId);
            }
            return t;
        }
        public void UpdateProceeding(Proceeding t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Proceeding>();
                int userId = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo().UserID;
                t.LastModifiedByID = userId > 0 ? userId : 1;
                t.LastModifiedDate = DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<ProceedingListItem> GetProceedingsPaged(int listType, int searchType, string searchText, int countyId, int rowOffset, int pageSize, string SortOrder, string sortDesc)
        {
            IEnumerable<ProceedingListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<ProceedingListItem>(System.Data.CommandType.StoredProcedure, "tjc_dcr_get_proceeding_list_paged", listType, searchType, searchText, countyId, rowOffset, pageSize, SortOrder, sortDesc);
            }
            return t;
        }
        public void DeleteCompletedRecords(int proceedingId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = string.Format("DELETE FROM tjc_dcr_accounting WHERE ProceedingID = {0}", proceedingId);
                ctx.Execute(System.Data.CommandType.Text, sql);
                sql = string.Format("DELETE FROM tjc_dcr_audio WHERE ProceedingID = {0}", proceedingId);
                ctx.Execute(System.Data.CommandType.Text, sql);
                sql = string.Format("DELETE FROM tjc_dcr_notification WHERE ProceedingID = {0}", proceedingId);
                ctx.Execute(System.Data.CommandType.Text, sql);
            }
        }

        public int GetProceedingsCount(int listType, int searchType, string searchText, int countyId)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_dcr_get_proceeding_list_count", listType, searchType, searchText, countyId);
            }
            return t;
        }
    }
}