using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace tjc.Modules.FamilySelfHelp.Services
{
    public class ClientNameController : DnnApiController
    {
        private readonly IHostSettings _hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();

        [HttpGet]
        [AllowAnonymous]
        [ActionName("GetClients")]
        public HttpResponseMessage GetClientNames(string name)
        {
            List<ViewModels.ClientNameViewModel> clientNames=new List<ViewModels.ClientNameViewModel>();

            var ctl = new Components.ClientController(_hostSettings);
            clientNames = ctl.GetClientNames(name).Select(clientName => new ViewModels.ClientNameViewModel(clientName)).ToList();

            return Request.CreateResponse(clientNames);
        }

    }
}
