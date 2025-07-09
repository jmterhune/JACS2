using tjc.Modules.jacs.Components;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services.Mappers
{
    internal class SiteUserMapper
    {
        public static SiteUserViewModel ToViewModel(SiteUser siteUser)
        {
            if (siteUser == null)
            {
                return null;
            }

            return new SiteUserViewModel(siteUser);
            
        }
    }
}