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
    [DnnAuthorize]
    public class LogEntryController : DnnApiController
    {
        [HttpGet]
        [DnnAuthorize]
        [ActionName("GetLogEntryByCaseNumber")]
        public HttpResponseMessage GetLogEntrysByCaseNumber(string caseNumber)
        {
            List<ViewModels.CourtCounsel.LogEntryViewModel> logEntries=new List<ViewModels.CourtCounsel.LogEntryViewModel>();

            var ctl = new Components.CourtCounsel.LogEntryController();
            logEntries = ctl.GetLogEntryByCaseNumber(caseNumber).Select(logEntry => new ViewModels.CourtCounsel.LogEntryViewModel(logEntry)).ToList();

            return Request.CreateResponse(logEntries);
        }

    }
}
