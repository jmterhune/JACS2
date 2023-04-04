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
    internal class OfficeLocationController
    {
        public void CreateOfficeLocation(OfficeLocation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<OfficeLocation>();
                rep.Insert(t);
            }
        }

        public void DeleteOfficeLocation(int officeLocationId)
        {
            var t = GetOfficeLocation(officeLocationId);
            DeleteOfficeLocation(t);
        }

        public void DeleteOfficeLocation(OfficeLocation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<OfficeLocation>();
                rep.Delete(t);
            }
        }

        public IEnumerable<OfficeLocation> GetOfficeLocations()
        {
            IEnumerable<OfficeLocation> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<OfficeLocation>();
                t = rep.Get();
            }
            return t;
        }

        public OfficeLocation GetOfficeLocation(int officeLocationId)
        {
            OfficeLocation t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<OfficeLocation>();
                t = rep.GetById(officeLocationId);
            }
            return t;
        }

        public void UpdateOfficeLocation(OfficeLocation t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<OfficeLocation>();
                rep.Update(t);
            }
        }

    }
}
