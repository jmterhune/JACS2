/*
' Copyright (c) 2021 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.ProcessServer.Components
{
    class ProcessServerController
    {
        // The application tables live in the jud12 (public) site's database.
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public ProcessServerController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateItem(ProcessServer t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProcessServer>();
                t.LastModifiedOnDate = System.DateTime.Now;
                t.CreatedOnDate = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteItem(int processServerId)
        {
            var t = GetItem(processServerId);
            DeleteItem(t);
        }

        public void DeleteItem(ProcessServer t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProcessServer>();
                var fCtl = new FileUploadController(_hostSettings);
                IEnumerable<UploadedFile> files = fCtl.GetFileUploadsByProcessServerId(t.ProcessServerID);
                foreach (UploadedFile file in files)
                {
                    fCtl.DeleteFileUpload(file.AttachmentID);
                }
                rep.Delete(t);
            }
        }

        public IEnumerable<ProcessServer> GetItems()
        {
            IEnumerable<ProcessServer> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProcessServer>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<ProcessServer> GetItemsByYear(int year)
        {
            IEnumerable<ProcessServer> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProcessServer>();
                t = rep.Find("Where ApplicationYear=@0", year).OrderByDescending(x=>x.LastModifiedOnDate);
            }
            return t;
        }

        public ProcessServer GetItem(int processServerId)
        {
            ProcessServer t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProcessServer>();
                t = rep.GetById(processServerId);
            }
            return t;
        }
        public ProcessServer GetItemByUserId(int userId,int year)
        {
            ProcessServer t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProcessServer>();
                t = rep.Find("Where UserId=@0 And ApplicationYear=@1", userId,year).OrderByDescending(x => x.CreatedOnDate).FirstOrDefault();
            }
            return t;
        }

        public void UpdateItem(ProcessServer t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProcessServer>();
                t.LastModifiedOnDate = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<int> GetApplicationYears()
        {
            IEnumerable<int> years;

            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                years = ctx.ExecuteQuery<int>(System.Data.CommandType.StoredProcedure, "tjc_process_servers_years");
            }
            if (!years.Contains(DateTime.Now.Year))
                years.Prepend(DateTime.Now.Year);
            return years;
        }

    }
}
