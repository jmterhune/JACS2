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
    internal class EEOController
    {
        public void CreateEEO(EEO t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EEO>();
                rep.Insert(t);
            }
        }

        public void DeleteEEO(long eeoId)
        {
            var t = GetEEO(eeoId);
            DeleteEEO(t);
        }

        public void DeleteEEO(EEO t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EEO>();
                rep.Delete(t);
            }
        }

        public IEnumerable<EEO> GetEEOs()
        {
            IEnumerable<EEO> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EEO>();
                t = rep.Get();
            }
            return t;
        }
      

        public EEO GetEEO(long eeoId)
        {
            EEO t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EEO>();
                t = rep.GetById(eeoId);
            }
            return t;
        }
        public IEnumerable<EEO> GetEmployeeEEOs(int jobGroupId)
        {
            IEnumerable<EEO> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EEO>();
                t = rep.Find("Where JobGroupId=@0",jobGroupId);
            }
            return t;
        }

        public void UpdateEEO(EEO t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EEO>();
                rep.Update(t);
            }
        }

    }
    internal class EEOListController {
        public IEnumerable<EeoListItem> GetEeoList()
        {
            IEnumerable<EeoListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EeoListItem>();
                t = rep.Get();
            }
            return t;
        }
    }
}
