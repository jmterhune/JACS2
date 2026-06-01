using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>REST endpoints for Service History on the Edit Employee page.
    /// Mirrors PositionsController — keyed by SSN under the hood, JS sees EmployeeId.</summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class ServicesController : DnnApiController
    {
        private readonly ServiceHistoryController _services = new ServiceHistoryController();
        private readonly EmployeeController _employees = new EmployeeController();

        [HttpGet]
        [ActionName("ForEmployee")]
        public HttpResponseMessage ForEmployee(int employeeId)
        {
            try
            {
                var emp = _employees.GetEmployee(employeeId);
                var ssn = emp?.SocialSecurityNumber;
                if (string.IsNullOrEmpty(ssn))
                    return Request.CreateResponse(HttpStatusCode.OK, new ServiceHistoryInfo[0]);
                return Request.CreateResponse(HttpStatusCode.OK, _services.GetForSsn(ssn));
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var item = _services.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(ServiceHistoryInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (string.IsNullOrWhiteSpace(item.SocialSecurityNumber))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "SocialSecurityNumber required");
            try
            {
                _services.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, ServiceHistoryInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                item.ServiceId = id;
                _services.Update(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try { _services.Delete(id); return Request.CreateResponse(HttpStatusCode.NoContent); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
