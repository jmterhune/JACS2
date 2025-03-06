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
using DotNetNuke.Collections;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;

namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class AttorneyController
    {
        public void CreateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Insert(t);
            }
        }
        public void DeleteAttorney(int attorneyId)
        {
            var t = GetAttorney(attorneyId);
            DeleteAttorney(t);
        }
        public void DeleteAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Attorney> GetAttorneys()
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<DropDownViewModel> GetAttorneyDropDownList()
        {
            IEnumerable<DropDownViewModel> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Get().Select(atty=> new DropDownViewModel { Id= atty.AttorneyID, Name=atty.ListName, Office=atty.OfficeName});
            }
            return t;
        }
        public Attorney GetAttorney(int attorneyId)
        {
            Attorney t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.GetById(attorneyId);
            }
            return t;
        }
        public void UpdateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Update(t);
            }
        }
        public IEnumerable<AttorneyViewModel> GetDesignationAttorneys(int designationId)
        {
            IEnumerable<AttorneyViewModel> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<AttorneyViewModel>(System.Data.CommandType.StoredProcedure, "tjc_rec_get_attorneys_by_designation", designationId);
            }
            return t;
        }
        public IEnumerable<Attorney> GetAttorneysByDesignation(int designationId)
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<Attorney>(System.Data.CommandType.StoredProcedure, "tjc_rec_get_attorneys_by_designation", designationId);
            }
            return t;
        }

    }
}
