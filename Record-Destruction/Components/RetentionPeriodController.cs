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
    internal class RetentionPeriodController
    {
        public void CreateRetentionPeriod(RetentionPeriod t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RetentionPeriod>();
                rep.Insert(t);
            }
        }

        public void DeleteRetentionPeriod(int retentionPeriodId)
        {
            var t = GetRetentionPeriod(retentionPeriodId);
            DeleteRetentionPeriod(t);
        }

        public void DeleteRetentionPeriod(RetentionPeriod t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RetentionPeriod>();
                rep.Delete(t);
            }
        }             
        public IEnumerable<RetentionPeriod> GetRetentionPeriods()
        {
            IEnumerable<RetentionPeriod> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RetentionPeriod>();
                t = rep.Get();
            }
            return t;
        }
        public RetentionPeriod GetRetentionPeriod(int retentionPeriodId)
        {
            RetentionPeriod t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RetentionPeriod>();
                t = rep.GetById(retentionPeriodId);
            }
            return t;
        }

        public void UpdateRetentionPeriod(RetentionPeriod t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RetentionPeriod>();
                rep.Update(t);
            }
        }
    }
}
