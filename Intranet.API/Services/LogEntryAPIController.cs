using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tjc.Intranet.API.Services
{
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
    public class LogEntryController : DnnApiController
    {
        [ActionName("GetLogEntryByCaseNumber")]
        public HttpResponseMessage GetLogEntrysByCaseNumber(string caseNumber)
        {
            List<ViewModels.CourtCounsel.LogEntryViewModel> logEntries=new List<ViewModels.CourtCounsel.LogEntryViewModel>();

            var ctl = new Components.CourtCounsel.LogEntryController();
            logEntries = ctl.GetLogEntrysByCaseNumber(caseNumber).Select(logEntry => new ViewModels.CourtCounsel.LogEntryViewModel(logEntry)).ToList();

            return Request.CreateResponse(logEntries);
        }

    }
}
