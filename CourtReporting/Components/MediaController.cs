using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtReporting.Components
{
   internal class MediaController
    {
        private readonly IHostSettings _hostSettings;

        public MediaController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateMedia(Media m)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Media>();
                rep.Insert(m);
            }
        }

        public void DeleteMedia(int mediaId)
        {
            var m = GetMedia(mediaId);

            DeleteMedia(m);
        }

        public void DeleteMedia(Media m)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Media>();
                rep.Delete(m);
            }
        }
        public Media GetMedia(int mediaId)
        {
            Media m;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Media>();
                m = rep.GetById(mediaId);
            }
            return m;
        }
        public Media GetMedia(MediaTypes mediaType)
        {
            Media m;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Media>();
                m = rep.Find("Where MediaTypeID=@0", (int)mediaType).FirstOrDefault();
            }
            return m;
        }

        public IEnumerable<Media> GetMedia()
        {
            IEnumerable<Media> m;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<Media>();
                m = rep.Get();
            }
            return m;
        }

    }
}