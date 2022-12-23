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
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtCounsel.Components
{
    internal class LogEntryListController
    {

        private const string CONN_INTRANET = "Intranet.API"; //Connection

        public IEnumerable<LogEntryListItem> GetLogEntryList()
        {
            IEnumerable<LogEntryListItem> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<LogEntryListItem>();
                t = rep.Get();
            }
            return t;
        }
        public LogEntryListItem GetLogEntryListItem(long logId)
        {
            LogEntryListItem t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<LogEntryListItem>();
                t = rep.GetById(logId);
            }
            return t;
        }

        public IEnumerable<LogEntryListItem> GetLogListItemsBySearchText(string searchText, SearchType type)
        {
            string sqlWhereClause = "";
            if (type == SearchType.caseName)
            {
                sqlWhereClause = string.Format("Where Description like @0", searchText);
            }
            else
            {
                sqlWhereClause = string.Format("Where CaseNumber like @0", searchText);
            }
            IEnumerable<LogEntryListItem> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<LogEntryListItem>();
                t = rep.Find(sqlWhereClause, searchText).OrderByDescending(x => x.DateReceived);
            }
            return t;
        }
        public IEnumerable<LogEntryListItem> GetLogListItemsByAttorney(long attorneyId, bool active, bool pending, bool closed)
        {
            string sqlWhereClause = string.Format("Where CurrentAttorneyId = {0}",attorneyId);
            if (active && !pending && !closed)
                sqlWhereClause += $" AND (StatusTypeId = {(int)StatusTypes.active})";
            else if (pending && !active && !closed)
                sqlWhereClause += $" AND (StatusTypeId = {(int)StatusTypes.pending})";
            else if (!pending && !active && closed)
                sqlWhereClause += $" AND (StatusTypeId = {(int)StatusTypes.closed})";
            else if (pending && active && !closed)
                sqlWhereClause += $" AND (StatusTypeId = {(int)StatusTypes.pending} OR StatusTypeId = {(int)StatusTypes.active})";
            else if (pending && !active && closed)
                sqlWhereClause += $" AND (StatusTypeId = {(int)StatusTypes.pending} OR StatusTypeId = {(int)StatusTypes.closed})";
            else if (!pending && active && closed)
                sqlWhereClause += $" AND (StatusTypeId = {(int)StatusTypes.active} OR StatusTypeId = {(int)StatusTypes.closed})";
            else if (pending && active && closed)
                sqlWhereClause += $" AND (StatusTypeId = {(int)StatusTypes.active} OR StatusTypeId = {(int)StatusTypes.closed} OR StatusTypeId = {(int)StatusTypes.pending})";
            IEnumerable<LogEntryListItem> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<LogEntryListItem>();
                t = rep.Find(sqlWhereClause).OrderByDescending(x => x.DateReceived);
            }
            return t;
        }
        public IEnumerable<LogEntryListItem> GetLogListItemsByUsername(string username)
        {
            
            IEnumerable<LogEntryListItem> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<LogEntryListItem>();
                t = rep.Find("Where (Email = @0 OR JudgeEmail = @0) AND (StatusTypeId IN (@1,@2))", username, (int)StatusTypes.active, (int)StatusTypes.pending).OrderByDescending(x => x.DateReceived);
            }
            return t;
        }

        public IEnumerable<LogEntryListItem> GetLogListItemsByLogId(long logId)
        {

            IEnumerable<LogEntryListItem> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<LogEntryListItem>();
                t = rep.Find("Where LogId = @0 ", logId).OrderByDescending(x => x.DateReceived);
            }
            return t;
        }
    }
}
