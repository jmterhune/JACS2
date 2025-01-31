/*
' Copyright (c) 2023 Joe Terhune
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

namespace tjc.Modules.RecordDestruction.Components
{
    internal class LogController
    {
        public void CreateLog(Log t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                rep.Insert(t);
            }
        }

        public void DeleteLog(int logId)
        {
            var t = GetLog(logId);
            DeleteLog(t);
        }

        public void DeleteLog(Log t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                rep.Delete(t);
            }
        }             
        public IEnumerable<Log> GetLogs()
        {
            IEnumerable<Log> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                t = rep.Get();
            }
            return t;
        }
        public Log GetLog(int logId)
        {
            Log t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                t = rep.GetById(logId);
            }
            return t;
        }

        public void UpdateLog(Log t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Log>();
                rep.Update(t);
            }
        }
        public IEnumerable<LogListItem> GetLogListItems()
        {
            IEnumerable<LogListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<LogListItem>();
                t = rep.Get();
            }
            return t;
        }
    }
}
