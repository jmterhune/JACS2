using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.ProSeLog.Components
{
    internal class HistoryController
    {
        public void CreateHistory(History t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<History>();
                rep.Insert(t);
            }
        }

        public void DeleteHistory(int historyId)
        {
            var t = GetHistory(historyId);
            DeleteHistory(t);
        }

        public void DeleteHistory(History t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<History>();
                rep.Delete(t);
            }
        }

        public IEnumerable<History> GetHistorys()
        {
            IEnumerable<History> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<History>();
                t = rep.Get();
            }
            return t;
        }

        public History GetHistory(int historyId)
        {
            History t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<History>();
                t = rep.GetById(historyId);
            }
            return t;
        }

        public void UpdateHistory(History t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<History>();
                rep.Update(t);
            }
        }

        public IEnumerable<HistoryListItem> GetHistoryListItems()
        {
            IEnumerable<HistoryListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryListItem>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<HistoryListItem> GetStats(int month, int year, int countyId)
        {
            IEnumerable<HistoryListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryListItem>();
                if (countyId == 0)
                    t = rep.Find("Where MonthNumber=@0 AND Year = @1", month, year);
                else
                    t = rep.Find("Where MonthNumber=@0 AND Year = @1 AND CountyID=@2", month, year, countyId);
            }
            return t;
        }
        public IEnumerable<HistoryListItem> GetHistoryListItemsByCaseNumber(string casenumber, int countyId)
        {
            IEnumerable<HistoryListItem> t;
            casenumber = string.Format("%{0}%", casenumber.Trim());
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryListItem>();
                if (countyId > 0)
                    t = rep.Find("Where CountyID=@0 AND Casenumber like @1", countyId, casenumber);
                else
                    t = rep.Find("Where Casenumber like @0", casenumber);
            }
            return t;
        }
        public IEnumerable<HistoryListItem> GetHistoryListItemsByPetitioner(string petitioner, int countyId)
        {
            IEnumerable<HistoryListItem> t;
            petitioner = string.Format("%{0}%", petitioner.Trim());
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryListItem>();
                if (countyId > 0)
                    t = rep.Find("Where CountyID=@0 AND Petitioner like @1", countyId, petitioner);
                else
                    t = rep.Find("Where Petitioner like @0", petitioner);

            }
            return t;
        }
        public IEnumerable<HistoryListItem> GetHistoryListItemsByRespondent(string respondent, int countyId)
        {
            IEnumerable<HistoryListItem> t;
            respondent = string.Format("%{0}%", respondent.Trim());
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryListItem>();
                if (countyId > 0)
                    t = rep.Find("Where CountyID=@0 AND Respondent like @1", countyId, respondent);
                else
                    t = rep.Find("Where Respondent like @0", respondent);
            }
            return t;
        }
        public IEnumerable<HistoryListItem> GetHistoryListItemsByCaseName(string casename, int countyId)
        {
            IEnumerable<HistoryListItem> t;
            casename = string.Format("%{0}%", casename.Trim());
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryListItem>();
                if (countyId > 0)
                    t = rep.Find("Where CountyID=@0 AND CaseName like @1", countyId, casename);
                else
                    t = rep.Find("Where CaseName like @0", casename);
            }
            return t;
        }
        public HistoryListItem GetHistoryListItem(int historyId)
        {
            HistoryListItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HistoryListItem>();
                t = rep.GetById(historyId);
            }
            return t;
        }
    }
}
