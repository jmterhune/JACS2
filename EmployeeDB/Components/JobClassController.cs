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
    internal class JobClassController
    {
        public void CreateJobClass(JobClass t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobClass>();
                rep.Insert(t);
            }
        }

        public void DeleteJobClass(int jobClassId)
        {
            var t = GetJobClass(jobClassId);
            DeleteJobClass(t);
        }

        public void DeleteJobClass(JobClass t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobClass>();
                rep.Delete(t);
            }
        }

        public IEnumerable<JobClass> GetJobClasses()
        {
            IEnumerable<JobClass> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobClass>();
                t = rep.Get();
            }
            return t;
        }

        public JobClass GetJobClass(int jobClassId)
        {
            JobClass t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobClass>();
                t = rep.GetById(jobClassId);
            }
            return t;
        }

        public void UpdateJobClass(JobClass t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobClass>();
                rep.Update(t);
            }
        }

    }
}
