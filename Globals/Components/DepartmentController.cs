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

namespace tjc.Modules.Globals
{
    public class DepartmentController
    {
        private const string CONN_INTRANET = "Intranet"; //Connection
        public void CreateDepartment(Department t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Department>();
                rep.Insert(t);
            }
        }

        public void DeleteDepartment(int departmentId)
        {
            var t = GetDepartment(departmentId);
            DeleteDepartment(t);
        }

        public void DeleteDepartment(Department t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Department>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Department> GetCounties()
        {
            IEnumerable<Department> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Department>();
                t = rep.Get();
            }
            return t;
        }
        public bool DepartmentExists(int departmentId)
        {
            Department t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Department>();
                t = rep.GetById(departmentId);
            }
            return t.DivisionId > 0;
        }
 
        public Department GetDepartment(int departmentId)
        {
            Department t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Department>();
                t = rep.GetById(departmentId);
            }
            return t;
        }
        public void UpdateDepartment(Department t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<Department>();
                rep.Update(t);
            }
        }
    }
}
