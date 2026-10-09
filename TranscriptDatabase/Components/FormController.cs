using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class FormController
    {
        private readonly IHostSettings _hostSettings;

        public FormController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateForm(Form t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Form>();
                rep.Insert(t);
            }
        }
        public void DeleteForm(int formId)
        {
            var t = GetForm(formId);
            DeleteForm(t);
        }
        public void DeleteForm(Form t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Form>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Form> GetForms()
        {
            IEnumerable<Form> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Form>();
                t = rep.Get();
            }
            return t;
        }
        public Form GetForm(int formId)
        {
            Form t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Form>();
                t = rep.GetById(formId);
            }
            return t;
        }
        public Form GetFormByType(DocumentTypes type)
        {
            Form t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Form>();
                t = rep.Find("Where DocumentTypeID=@0",(int)type).FirstOrDefault();
            }
            return t;
        }
        public void UpdateForm(Form t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Form>();
                rep.Update(t);
            }
        }
    }
}