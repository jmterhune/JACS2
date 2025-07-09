using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class TemplateTimeslotController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateTemplateTimeslot(TemplateTimeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                rep.Insert(t);
            }
        }
        public void DeleteTemplateTimeslot(int templatetimeslotId)
        {
            var t = GetTemplateTimeslot(templatetimeslotId);
            DeleteTemplateTimeslot(t);
        }
        public void DeleteTemplateTimeslot(TemplateTimeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                rep.Delete(t);
            }
        }
        public IEnumerable<TemplateTimeslot> GetTemplateTimeslots()
        {
            IEnumerable<TemplateTimeslot> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                t = rep.Get();
            }
            return t;
        }
        public TemplateTimeslot GetTemplateTimeslot(int templatetimeslotId)
        {
            TemplateTimeslot t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                t = rep.GetById(templatetimeslotId);
            }
            return t;
        }
        public void UpdateTemplateTimeslot(TemplateTimeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                rep.Update(t);
            }
        }
    }
}