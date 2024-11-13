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

namespace tjc.Modules.CourtCounsel.Components
{
    internal class LogEntryController
    {
        

        public void CreateLogEntry(LogEntry t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<LogEntry>();
                rep.Insert(t);
            }
        }

        public void DeleteLogEntry(long logEntryId)
        {
            var t = GetLogEntry(logEntryId);
            DeleteLogEntry(t);
        }

        public void DeleteLogEntry(LogEntry t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<LogEntry>();
                rep.Delete(t);
            }
        }

        public IEnumerable<LogEntry> GetLogEntrys()
        {
            IEnumerable<LogEntry> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<LogEntry>();
                t = rep.Get();
            }
            return t;
        }
        public LogEntry GetLogEntry(long logEntryId)
        {
            LogEntry t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<LogEntry>();
                t = rep.GetById(logEntryId);
            }
            return t;
        }
        public void UpdateLogEntry(LogEntry t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<LogEntry>();
                rep.Update(t);
            }
        }
    }
}
