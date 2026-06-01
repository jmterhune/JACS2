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
using tjc.Modules.ExpertWitness.Components;

namespace tjc.Modules.ExpertWitness.Components
{
    internal class TemplateController
    {
        public void CreateTemplate(Template t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Template>();
                rep.Insert(t);
            }
        }

        public void DeleteTemplate(int templateId)
        {
            var t = GetTemplate(templateId);
            DeleteTemplate(t);
        }

        public void DeleteTemplate(Template t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Template>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Template> GetTemplates()
        {
            IEnumerable<Template> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Template>();
                t = rep.Get();
            }
            return t;
        }

        public Template GetTemplate(int templateId)
        {
            Template t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Template>();
                t = rep.GetById(templateId);
            }
            return t;
        }

        public void UpdateTemplate(Template t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Template>();
                rep.Update(t);
            }
        }
        public IEnumerable<TemplateSequence> GetTemplateSequences(int templateId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
              return  ctx.ExecuteQuery<TemplateSequence>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_template_sequences",templateId);
            }
        }
        public TemplateSequence GetTemplateSequence(int templateId,int sequence)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteSingleOrDefault<TemplateSequence>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_template_sequence_by_sequence", templateId,sequence);
            }
        }
        public IEnumerable<TemplateType> GetTemplateTypesByTemplate(int templateid)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<TemplateType>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_template_types",templateid);
            }
        }
        public IEnumerable<Type> GetTemplateTypeTypesBySequence(int templateId, int sequence)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Type>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_types_by_template_by_sequence",templateId, sequence);
            }
        }
        public IEnumerable<TemplateType> GetTemplateTypesBySequence(int templateId, int sequence)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<TemplateType>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_template_types_by_sequence", templateId, sequence);
            }
        }
       
        public IEnumerable<Type> GetTemplateTypeTypes(int templateId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Type>(System.Data.CommandType.StoredProcedure, "tjc_expert_get_template_type_types",templateId);
            }
        }
        public void CreateTemplateSequence(int templateId, int sequence,int numberRequired)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_create_template_sequence",  templateId,sequence,numberRequired);
            }
        }
        public void UpdateTemplateSequence(int templateId, int sequence, int numberRequired)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_create_template_sequence", templateId, sequence, numberRequired);
            }
        }
        public void DeleteTemplateSequences(int templateId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_delete_template_sequence", templateId);
            }
        }
        public void CreatTemplateType(int templateId,int typeId, int sequence)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_create_template_type", templateId, typeId,sequence);
            }
        }
        public void UpdateTemplateType(int templateId, int typeId, int sequence,int newSequence)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_update_template_type", templateId, typeId, sequence,newSequence);
            }
        }
        public void DeleteTemplateTypes(int templateId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_expert_delete_template_type", templateId);
            }
        }
    }
}
