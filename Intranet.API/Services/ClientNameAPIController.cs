using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace tjc.Intranet.API.Services.FamilySelfHelp
{
    public class ClientNameController : DnnApiController
    {
       
        [HttpGet]
        [ActionName("client")]
        [AllowAnonymous]
        public List<ViewModels.FamilySelfHelp.ClientNameViewModel> GetClients(string name)
        {

            try
            {
                List<ViewModels.FamilySelfHelp.ClientNameViewModel> clientNames = new List<ViewModels.FamilySelfHelp.ClientNameViewModel>();

                var ctl = new Components.FamilySelfHelp.ClientController();
                clientNames = ctl.GetClientNames(name).Select(clientName => new ViewModels.FamilySelfHelp.ClientNameViewModel(clientName)).ToList();
                return clientNames;
            }
            catch (System.Exception ex)
            {

                Exceptions.LogException(ex);
                return null;
            }
        }

    }
}
