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
    internal class HearingController
    {
        public void CreateHearing(HearingLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingLog>();
                rep.Insert(t);
            }
        }

        public void DeleteHearing(int logId)
        {
            var t = GetHearing(logId);
            DeleteHearing(t);
        }

        public void DeleteHearing(HearingLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingLog>();
                rep.Delete(t);
            }
        }

        public IEnumerable<HearingLog> GetHearings()
        {
            IEnumerable<HearingLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingLog>();
                t = rep.Get();
            }
            return t;
        }

        public HearingLog GetHearing(int logId)
        {
            HearingLog t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingLog>();
                t = rep.GetById(logId);
            }
            return t;
        }

        public void UpdateHearing(HearingLog t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingLog>();
                rep.Update(t);
            }
        }
        public IEnumerable<HearingLog> GetHearingLogPaged(int userId,int status,DateTime cutoffDate, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<HearingLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<HearingLog>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_log_paged", userId, status,cutoffDate, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public IEnumerable<HearingLog> GetHearingLogPaged(int userId, int status, DateTime cutoffDate,string searchText, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<HearingLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<HearingLog>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_log_paged_search", userId, status, cutoffDate,searchText, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }

        public int GetHearingLogCount(int userId, int status, DateTime cutoffDate)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_log_count", userId, status,cutoffDate);
            }
            return t;
        }
        public int GetHearingLogCount(int userId, int status, DateTime cutoffDate, string searchText)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_log_count_search", userId, status, cutoffDate,searchText);
            }
            return t;
        }
        #region "Chief Judges"
        public IEnumerable<HearingLog> GetHearingLogPaged(int userId, int status, DateTime cutoffDate, string searchText,int judgeUserId, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<HearingLog> t;
            using (IDataContext ctx = DataContext.Instance())
            {

                t = ctx.ExecuteQuery<HearingLog>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_log_paged_chief_judge", userId, status, cutoffDate, searchText,judgeUserId, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public int GetHearingLogCount(int userId, int status, DateTime cutoffDate, string searchText,int judgeUserId)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_hearing_get_log_count_chief_judge", userId, status, cutoffDate, searchText,judgeUserId);
            }
            return t;
        }
        #endregion
    }
}
