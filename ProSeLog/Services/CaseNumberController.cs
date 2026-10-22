using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.ProSeLog.Components.Services.ViewModels;
namespace tjc.Modules.ProSeLog.Components.Services
{
    public class CaseNumberController : DnnApiController
    {
        private readonly IHostSettings _hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();

        [HttpGet]
        [AllowAnonymous]
        [ActionName("GetCaseNumbers")]
        public HttpResponseMessage GetCaseNumbers(string casenumber)
        {
            List<CaseNumberViewModel> caseNumbers=new List<CaseNumberViewModel>();
            var ctl = new CaseTypeController(_hostSettings);
            caseNumbers = ctl.GetCaseNumbers(casenumber).Select(caseNumber => new CaseNumberViewModel(caseNumber)).ToList();
            return Request.CreateResponse(caseNumbers);
        }
    }
}
