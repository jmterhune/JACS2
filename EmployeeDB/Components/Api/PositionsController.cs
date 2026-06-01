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
    /// <summary>REST endpoints for Position History on the Edit Employee page.
    /// The underlying DB key is SocialSecurityNumber, but the JS layer only knows
    /// EmployeeId — ForEmployee resolves that internally.</summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class PositionsController : DnnApiController
    {
        private readonly PositionHistoryController _positions = new PositionHistoryController();
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
                    return Request.CreateResponse(HttpStatusCode.OK, new PositionHistoryInfo[0]);
                return Request.CreateResponse(HttpStatusCode.OK, _positions.GetForSsn(ssn));
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var item = _positions.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(PositionHistoryInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (string.IsNullOrWhiteSpace(item.SocialSecurityNumber))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "SocialSecurityNumber required");
            try
            {
                _positions.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, PositionHistoryInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                item.PositionId = id;
                _positions.Update(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try { _positions.Delete(id); return Request.CreateResponse(HttpStatusCode.NoContent); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
