using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    internal class AudioController
    {
        public void CreateAudio(Audio t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Audio>();
                rep.Insert(t);
            }
        }
        public void DeleteAudio(int audioId)
        {
            var t = GetAudio(audioId);
            DeleteAudio(t);
        }
        public void DeleteAudio(Audio t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Audio>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Audio> GetAudios()
        {
            IEnumerable<Audio> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Audio>();
                t = rep.Get();
            }
            return t;
        }
        public Audio GetAudio(int audioId)
        {
            Audio t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Audio>();
                t = rep.GetById(audioId);
            }
            return t;
        }
        public IEnumerable<Audio> GetAudiosByProceeding(int proceedingId)
        {
            IEnumerable<Audio> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Audio>();
                t = rep.Find("Where ProceedingID = @0",proceedingId);
            }
            return t;
        }
        public void UpdateAudio(Audio t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Audio>();
                rep.Update(t);
            }
        }
    }
}