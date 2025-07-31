using DotNetNuke.Data;
using System;
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
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;

                var rep = ctx.GetRepository<TemplateTimeslot>();
                rep.Insert(t);
            }
        }
        public void DeleteTemplateTimeslot(long templatetimeslotId)
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
        
        public TemplateTimeslot GetTemplateTimeslot(long templatetimeslotId)//court_template_id
        {
            TemplateTimeslot t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                t = rep.GetById(templatetimeslotId);
            }
            return t;
        }
        public IEnumerable<TemplateTimeslot> GetTemplateTimeslotsByTemplateId(long courtTemplateId)
        {
            IEnumerable<TemplateTimeslot> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TemplateTimeslot>();
                t = rep.Find("Where court_template_id=@0",courtTemplateId);
            }
            return t;
        }
        public void UpdateTemplateTimeslot(TemplateTimeslot t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t.updated_at = DateTime.Now;
                var rep = ctx.GetRepository<TemplateTimeslot>();
                rep.Update(t);
            }
        }
    }
}