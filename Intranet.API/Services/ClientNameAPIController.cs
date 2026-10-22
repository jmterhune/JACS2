using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace tjc.Intranet.API.Services.FamilySelfHelp
{
    public class ClientNameController : DnnApiController
    {
        private readonly IHostSettings _hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();

        [HttpGet]
        [ActionName("client")]
        [AllowAnonymous]
        public List<ViewModels.FamilySelfHelp.ClientNameViewModel> GetClients(string name)
        {

            try
            {
                List<ViewModels.FamilySelfHelp.ClientNameViewModel> clientNames = new List<ViewModels.FamilySelfHelp.ClientNameViewModel>();

                var ctl = new Components.FamilySelfHelp.ClientController(_hostSettings);
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
