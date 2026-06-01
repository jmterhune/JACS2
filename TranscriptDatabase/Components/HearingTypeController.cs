using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class HearingTypeController
    {
        public void CreateHearingType(HearingType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingType>();
                rep.Insert(t);
            }
        }
        public void DeleteHearingType(int hearingtypeId)
        {
            var t = GetHearingType(hearingtypeId);
            DeleteHearingType(t);
        }
        public void DeleteHearingType(HearingType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingType>();
                rep.Delete(t);
            }
        }
        public IEnumerable<HearingType> GetHearingTypes()
        {
            IEnumerable<HearingType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingType>();
                t = rep.Get();
            }
            return t;
        }
        public HearingType GetHearingType(int hearingtypeId)
        {
            HearingType t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingType>();
                t = rep.GetById(hearingtypeId);
            }
            return t;
        }
        public void UpdateHearingType(HearingType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<HearingType>();
                rep.Update(t);
            }
        }
    }
}