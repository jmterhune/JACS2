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

namespace tjc.Modules.CourtCounsel.Components
{
    internal class TimeSpanController
    {
        

        public void CreateTimeSpan(TimeSpan t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSpan>();
                rep.Insert(t);
            }
        }

        public void DeleteTimeSpan(int timeSpanId)
        {
            var t = GetTimeSpan(timeSpanId);
            DeleteTimeSpan(t);
        }

        public void DeleteTimeSpan(TimeSpan t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSpan>();
                rep.Delete(t);
            }
        }

        public IEnumerable<TimeSpan> GetTimeSpans()
        {
            IEnumerable<TimeSpan> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSpan>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<TimeSpan> GetTimeSpans(bool active)
        {
            IEnumerable<TimeSpan> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSpan>();
                t = rep.Find("Where Active=1").OrderBy(x=>x.TimeSpanName);
            }
            return t;
        }

        public TimeSpan GetTimeSpan(int timeSpanId)
        {
            TimeSpan t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSpan>();
                t = rep.GetById(timeSpanId);
            }
            return t;
        }
        public void UpdateTimeSpan(TimeSpan t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TimeSpan>();
                rep.Update(t);
            }
        }
    }
}
