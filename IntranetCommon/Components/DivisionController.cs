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

namespace tjc.Modules.IntranetCommon.Components
{
    public class DivisionController
    {
        public void CreateDivision(Division t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Division>();
                rep.Insert(t);
            }
        }

        public void DeleteDivision(int divisionId)
        {
            var t = GetDivision(divisionId);
            DeleteDivision(t);
        }

        public void DeleteDivision(Division t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Division>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Division> GetDivisions()
        {
            IEnumerable<Division> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Division>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Division> GetDivisions(bool active)
        {
            IEnumerable<Division> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Division>();
                t = rep.Find("Where Active=1").OrderBy(x=>x.DivisionName);
            }
            return t;
        }

        public Division GetDivision(int divisionId)
        {
            Division t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Division>();
                t = rep.GetById(divisionId);
            }
            return t;
        }
        public void UpdateDivision(Division t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Division>();
                rep.Update(t);
            }
        }
    }
}
