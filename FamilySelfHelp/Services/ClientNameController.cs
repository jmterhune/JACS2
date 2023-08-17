using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace tjc.Modules.FamilySelfHelp.Services
{
    public class ClientNameController : DnnApiController
    {
        [HttpGet]
        [AllowAnonymous]
        [ActionName("GetClients")]
        public HttpResponseMessage GetClientNames(string name)
        {
            List<ViewModels.ClientNameViewModel> clientNames=new List<ViewModels.ClientNameViewModel>();

            var ctl = new Components.ClientController();
            clientNames = ctl.GetClientNames(name).Select(clientName => new ViewModels.ClientNameViewModel(clientName)).ToList();

            return Request.CreateResponse(clientNames);
        }

    }
}
