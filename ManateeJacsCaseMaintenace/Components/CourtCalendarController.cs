/*
' Copyright (c) 2023 jterhune
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
using System.Linq;

namespace tjc.Modules.JacsCaseMaint.Components
{
    internal class CourtCalendarController
    {
        private const string CONN_INTRANET = "jacsManatee";

        public IEnumerable<CourtCalendar> GetCourtCalendars(string year, string caseType, string sequence)
        {
            IEnumerable<CourtCalendar> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                string caseNumber = string.Format("%{0}%{1}%{2}%", year, caseType, sequence);
                var rep = ctx.GetRepository<CourtCalendar>();
                t = rep.Find("Where CASENUM like @0", caseNumber);
            }
            return t;
        }

        public CourtCalendar GetCourtCalendar(string courtCode,DateTime calDate,string timeFrom,int timeSlotNum)
        {
            CourtCalendar t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<CourtCalendar>();
                t = rep.Find("Where COURTCODE=@0 And CALDATE=@1 And TIMEFROM=@2 And TIMESLOTNUM=@3", courtCode,calDate,timeFrom,timeSlotNum).FirstOrDefault();
            }
            return t;
        }

        public void DeleteReferral(string courtCode, DateTime calDate, string timeFrom, int timeSlotNum)
        {
            var t = GetCourtCalendar(courtCode, calDate, timeFrom, timeSlotNum);
            DeleteReferral(t);
        }

        public void DeleteReferral(CourtCalendar t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<CourtCalendar>();
                rep.Delete(t);
            }
        }
    }
}
