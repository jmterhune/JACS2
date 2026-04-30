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
    internal class JobGroupController
    {
        public void CreateJobGroup(JobGroup t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobGroup>();
                rep.Insert(t);
            }
        }

        public void DeleteJobGroup(int jobGroupID)
        {
            var t = GetJobGroup(jobGroupID);
            DeleteJobGroup(t);
        }

        public void DeleteJobGroup(JobGroup t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobGroup>();
                rep.Delete(t);
            }
        }

        public IEnumerable<JobGroup> GetJobGroups()
        {
            IEnumerable<JobGroup> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobGroup>();
                t = rep.Get();
            }
            return t;
        }

        public JobGroup GetJobGroup(int jobGroupID)
        {
            JobGroup t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobGroup>();
                t = rep.GetById(jobGroupID);
            }
            return t;
        }

        public void UpdateJobGroup(JobGroup t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<JobGroup>();
                rep.Update(t);
            }
        }

    }
}
