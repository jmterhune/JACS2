/*
' Copyright (c) 2024 Joe Terhune
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

namespace tjc.Modules.HearingLog.Components
{
    internal class CourtCounselController
    {

        public IEnumerable<CourtCounselLog> GetCourtCounselLogs()
        {
            IEnumerable<CourtCounselLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtCounselLog>();
                t = rep.Get();
            }
            return t;
        }

        public CourtCounselLog GetCourtCounselLog(int logId)
        {
            CourtCounselLog t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourtCounselLog>();
                t = rep.GetById(logId);
            }
            return t;
        }


        public IEnumerable<CourtCounselLog> GetCourtCounselLogPaged(int userId, DateTime startDate, DateTime endDate, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<CourtCounselLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<CourtCounselLog>(System.Data.CommandType.StoredProcedure, "tjc_court_counsel_get_log_paged", userId, startDate, endDate, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public IEnumerable<CourtCounselLog> GetCourtCounselLogPaged(int userId, DateTime startDate, DateTime endDate, string searchText, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<CourtCounselLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<CourtCounselLog>(System.Data.CommandType.StoredProcedure, "tjc_court_counsel_get_log_paged_search", userId, startDate, endDate, searchText, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }

        public int GetCourtCounselLogCount(int userId, DateTime startDate, DateTime endDate)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_court_counsel_get_log_count", userId, startDate, endDate);
            }
            return t;
        }
        public int GetCourtCounselLogCount(int userId, DateTime startDate, DateTime endDate, string searchText)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_court_counsel_get_log_count_search", userId, startDate, endDate, searchText);
            }
            return t;
        }

        #region "Chief Judges"
        public IEnumerable<CourtCounselLog> GetCourtCounselLogPaged(DateTime startDate, DateTime endDate, string searchText, int judgeUserId, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<CourtCounselLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<CourtCounselLog>(System.Data.CommandType.StoredProcedure, "tjc_court_counsel_get_log_paged_chief_judge",  startDate, endDate, searchText, judgeUserId, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public int GetCourtCounselLogCount(DateTime startDate, DateTime endDate, string searchText, int judgeUserId)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_court_counsel_get_log_count_chief_judge",  startDate, endDate, searchText, judgeUserId);
            }
            return t;
        }

        #endregion
    }
}
