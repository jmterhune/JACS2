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
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Intranet.API.Components.CourtCounsel
{
    internal class LogEntryController
    {
        public IEnumerable<LogEntry> GetLogEntryByCaseNumber(string caseNumber)
        {
            IEnumerable<LogEntry> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<LogEntryListItem>();
                //t = rep.Find("Where CaseNumber Like @0", caseNumber.Trim() + "%").Select(x => new LogEntry { LogId = x.LogId, CaseNumber = x.CaseNumber, Description = x.Description, IsCase = true, CountyId = x.CountyId }).Distinct().ToList();
                t = ctx.ExecuteQuery<LogEntry>(System.Data.CommandType.StoredProcedure, "court_counsel_get_matching_casenumbers", caseNumber.Trim() + "%");

            }
            return t;
        }
    }
}
