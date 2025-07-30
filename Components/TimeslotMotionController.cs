using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.jacs.Components
{
    internal class TimeslotMotionController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateTimeslotMotion(TimeslotMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                rep.Insert(t);
            }
        }
        public void DeleteTimeslotMotion(long timeslotmotionId)
        {
            var t = GetTimeslotMotion(timeslotmotionId);
            DeleteTimeslotMotion(t);
        }
        public void DeleteTimeslotMotion(TimeslotMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {

                var rep = ctx.GetRepository<TimeslotMotion>();
                rep.Delete(t);
            }
        }
        public void DeleteTimeslotMotions(long timeslotId, string templateType)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                IEnumerable<TimeslotMotion> timeslots = GetTemplateTimeslotMotions(timeslotId, templateType);
                foreach (var t in timeslots)
                {
                    DeleteTimeslotMotion(t);
                }
            }
        }
        public void DeleteTimeslotMotion(long motionId,long timeslotId, string templateType)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                TimeslotMotion timeslot = GetTemplateTimeslotMotion(motionId,timeslotId, templateType);
                    DeleteTimeslotMotion(timeslot);
            }
        }
        public IEnumerable<TimeslotMotion> GetTimeslotMotions()
        {
            IEnumerable<TimeslotMotion> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<TimeslotMotion> GetTemplateTimeslotMotions(long timeslotId,string templateType)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
               
                    var query = @"
                    SELECT *
                    FROM [timeslot_motions]
                    WHERE timeslotable_id = @0 AND timeslotable_type = @1";
                    return ctx.ExecuteQuery<TimeslotMotion>(System.Data.CommandType.Text, query, timeslotId,templateType);
            }
        }
        public TimeslotMotion GetTemplateTimeslotMotion(long motionId,long timeslotId, string templateType)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT *
                    FROM [timeslot_motions]
                    WHERE motion_id=@0 AND timeslotable_id = @1 AND timeslotable_type = @2";
                return ctx.ExecuteQuery<TimeslotMotion>(System.Data.CommandType.Text, query, timeslotId, templateType).FirstOrDefault();
            }
        }
        public IEnumerable<Motion> GetMotionsByTemplateTimeslot(long timeslotId)
        {
            IEnumerable<Motion> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var query = @"
                    SELECT m.*
                    FROM [motions] m
                    INNER JOIN [timeslot_motions] tm ON m.id = tm.motion_id
                    WHERE tm.timeslotable_type = 'App\Models\TemplateTimeslot' AND tm.timeslotable_id = @0";
                return ctx.ExecuteQuery<Motion>(System.Data.CommandType.Text, query, timeslotId);
            }
        }
        public TimeslotMotion GetTimeslotMotion(long timeslotmotionId)
        {
            TimeslotMotion t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                t = rep.GetById(timeslotmotionId);
            }
            return t;
        }
        public TimeslotMotion GetTimeslotMotion(long motionId,long timeslotId,string templateTimeslotType)
        {
            TimeslotMotion t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                t = rep.Find("Where motion_id = @0 AND timeslotable_id=@1 and timeslotable_type=@2").FirstOrDefault();
            }
            return t;
        }
        public void UpdateTimeslotMotion(TimeslotMotion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<TimeslotMotion>();
                rep.Update(t);
            }
        }
    }
}