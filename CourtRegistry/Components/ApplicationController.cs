/*
' Copyright (c) 2025 Joe Terhune
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

namespace tjc.Modules.CourtRegistry.Components
{
    internal class ApplicationController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateApplication(Application t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                rep.Insert(t);
            }
        }

        public void DeleteApplication(int applicationId)
        {
            var t = GetApplication(applicationId);
            DeleteApplication(t);
        }

        public void DeleteApplication(Application t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Application> GetApplications()
        {
            IEnumerable<Application> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                t = rep.Get();
            }
            return t;
        }

        public Application GetApplication(int applicationId)
        {
            Application t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                t = rep.GetById(applicationId);
            }
            return t;
        }

        public void UpdateApplication(Application t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Application>();
                rep.Update(t);
            }
        }
        //Application Periods
        public void CreateApplicationPeriod(ApplicationPeriod t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                rep.Insert(t);
            }
        }

        public void DeleteApplicationPeriod(int applicationYear)
        {
            var t = GetApplicationPeriod(applicationYear);
            DeleteApplicationPeriod(t);
        }

        public void DeleteApplicationPeriod(ApplicationPeriod t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ApplicationPeriod> GetApplicationPeriods()
        {
            IEnumerable<ApplicationPeriod> t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                t = rep.Get();
            }
            return t;
        }

        public ApplicationPeriod GetApplicationPeriod(int applicationYear)
        {
            ApplicationPeriod t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                t = rep.GetById(applicationYear);
            }
            return t;
        }

        public void UpdateApplicationPeriod(ApplicationPeriod t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ApplicationPeriod>();
                rep.Update(t);
            }
        }
    }
}
