using System.Collections.Generic;
using Dnn.PersonaBar.Library.Controllers;
using Dnn.PersonaBar.Library.Model;
using DotNetNuke.Entities.Users;

namespace Tjc.Modules.ConfigEncryption.Components
{
    public class MenuController : IMenuItemController
    {
        public void UpdateParameters(MenuItem menuItem)
        {
        }

        // Encrypting web.config is a host-level operation.
        public bool Visible(MenuItem menuItem)
        {
            var user = UserController.Instance.GetCurrentUserInfo();
            return user != null && user.IsSuperUser;
        }

        public IDictionary<string, object> GetSettings(MenuItem menuItem)
        {
            return new Dictionary<string, object>
            {
                { "uiUrl", "/DesktopModules/Admin/Dnn.PersonaBar/Modules/Tjc.ConfigEncryption" },
            };
        }
    }
}
