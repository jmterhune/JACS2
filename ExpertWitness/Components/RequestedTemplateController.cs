/*
' Copyright (c) 2024 Joe Terhune
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
using System.Web.DynamicData;
using tjc.Modules.ExpertWitness.Components;

namespace tjc.Modules.ExpertWitness.Components
{
    internal class RequestedTemplateController
    {
        public void CreateRequestedTemplate(RequestCart t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                rep.Insert(t);
            }
        }

        public void DeleteRequestedTemplate(int requestId)
        {
            var t = GetRequestedTemplate(requestId);
            DeleteRequestedTemplate(t);
        }

        public void DeleteRequestedTemplate(RequestCart t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                rep.Delete(t);
            }
        }


        public IEnumerable<RequestCart> GetRequestedTemplates()
        {
            IEnumerable<RequestCart> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<RequestCart> GetRequestedTemplates(Guid guid)
        {
            IEnumerable<RequestCart> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                t = rep.Find("Where Guid=@0",guid);
            }
            return t;
        }
       
        public IEnumerable<RequestCart> GetRequestedTemplatesByGuidByStatus(Guid guid,int status)
        {
            IEnumerable<RequestCart> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                t = rep.Find("Where Guid=@0 AND Status=@1",guid,status);
            }
            return t;
        }
        public RequestCart GetRequestedTemplatesByExpertByGuidBySequence(int expertId,Guid guid, int sequence)
        {
            RequestCart t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                t = rep.Find("Where Guid=@0 AND Sequence=@1 AND ExpertID=@2",guid, sequence, expertId).FirstOrDefault();
            }
            return t;
        }
        public IEnumerable<RequestedTemplate> GetRequestedTemplatesByGuid(Guid guid)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<RequestedTemplate>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_requested_template_by_guid", guid);
            }
        }
        public IEnumerable<RequestedTemplate> GetRequestedTemplatesByTemplateByLocationBySequence(int templateId,int locationId, int sequence)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<RequestedTemplate>(System.Data.CommandType.StoredProcedure, "tjc_expert_requested_template_by_template_by_location_by_sequence", templateId,locationId,sequence);
            }
        }
        public IEnumerable<RequestCart> GetRequestedTemplatesByGuidBySequence(Guid guid, int sequence)
        {
            IEnumerable<RequestCart> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                t = rep.Find("Where Guid=@0 AND Sequence=@1", guid, sequence);
            }
            return t;
        }
        public RequestCart GetRequestedTemplate(int requestId)
        {
            RequestCart t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                t = rep.GetById(requestId);
            }
            return t;
        }

        public void UpdateRequestedTemplate(RequestCart t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestCart>();
                rep.Update(t);
            }
        }
        public void DeleteRequestTemplatesByGuid(Guid guid)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_delete_request_template_by_guid", guid);
            }
        }
        public IEnumerable<RequestedTemplate> GetTemporaryRequestedTemplates(int templateid,int sequence,int locationId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
              return  ctx.ExecuteQuery<RequestedTemplate>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_requested_template_temp", templateid,sequence,locationId);
            }
        }

    }
}
