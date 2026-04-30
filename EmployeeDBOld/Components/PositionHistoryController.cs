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

namespace tjc.Modules.EmployeeDB.Components
{
    internal class PositionHistoryController
    {
        public void CreatePositionHistory(PositionHistory t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PositionHistory>();
                rep.Insert(t);
            }
        }

        public void DeletePositionHistory(int positionHistoryId)
        {
            var t = GetPositionHistory(positionHistoryId);
            DeletePositionHistory(t);
        }

        public void DeletePositionHistory(PositionHistory t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PositionHistory>();
                rep.Delete(t);
            }
        }

        public IEnumerable<PositionHistory> GetPositionHistories()
        {
            IEnumerable<PositionHistory> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PositionHistory>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<PositionHistory> GetPositionHistoriesByEmployee(string ssn)
        {
            IEnumerable<PositionHistory> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PositionHistory>();
                t = rep.Find("Where SocialSecurityNumber = @0",ssn);
            }
            return t;
        }

        public PositionHistory GetPositionHistory(int positionHistoryId)
        {
            PositionHistory t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PositionHistory>();
                t = rep.GetById(positionHistoryId);
            }
            return t;
        }

        public void UpdatePositionHistory(PositionHistory t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PositionHistory>();
                rep.Update(t);
            }
        }

    }
}
