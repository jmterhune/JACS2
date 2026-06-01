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
using System.Collections.Generic;

namespace tjc.Modules.ExpertWitness.Components
{
    internal class ExpertController
    {
        public void CreateExpert(Expert t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Expert>();
                rep.Insert(t);
            }
        }

        public void DeleteExpert(int expertId)
        {
            var t = GetExpert(expertId);
            DeleteExpert(t);
        }

        public void DeleteExpert(Expert t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Expert>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Expert> GetExperts()
        {
            IEnumerable<Expert> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Expert>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<ExpertRequestListItem> GetExpertRequestListItems(int requestId)
        {
            IEnumerable<ExpertRequestListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExpertRequestListItem>();
                t = rep.Find("Where RequestID=@0",requestId);
            }
            return t;
        }

        public Expert GetExpert(int expertId)
        {
            Expert t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Expert>();
                t = rep.GetById(expertId);
            }
            return t;
        }

        public void UpdateExpert(Expert t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Expert>();
                rep.Update(t);
            }
        }
        public IEnumerable<Location> GetExpertLocationLocations(int expertId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Location>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_expert_location_locations",expertId);
            }
        }
        public void DeleteExpertLocations(int expertId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                 ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_delete_expert_locations", expertId);
            }
        }

        public IEnumerable<Type> GetExpertTypeTypes(int expertId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Type>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_expert_type_types",expertId);
            }
        }
        public IEnumerable<ExpertTemplate> GetExpertTemplates(int templateId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<ExpertTemplate>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_expert_templates",  templateId);
            }
        }

        public ExpertTemplate GetExpertTemplate(int expertId,int templateId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteSingleOrDefault<ExpertTemplate>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_expert_template", expertId,templateId);
            }
        }
        public void DeleteExpertTypes(int expertId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_delete_expert_types", expertId);
            }
        }
        public IEnumerable<Template> GetExpertTemplateTemplates(int expertId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Template>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_expert_template_templates", expertId);
            }
        }
        public void DeleteExpertTemplates(int expertId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_delete_expert_templates", expertId);
            }
        }
        public void CreateExpertTemplate(int expertId,int templateId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_create_expert_template", expertId,templateId);
            }
        }
        public void CreateExpertType(int expertId, int typeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_create_expert_type", expertId, typeId);
            }
        }
        public void CreateExpertLocation(int expertId, int locationId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_create_expert_location", expertId, locationId);
            }
        }
        public void CreateExpertRequest(int expertId,int requestId, int sequence)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_create_expert_request", expertId, requestId,sequence);
            }
        }
    }
}
