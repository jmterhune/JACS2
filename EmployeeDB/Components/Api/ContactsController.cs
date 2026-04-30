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
    /// <summary>REST endpoints for Emergency Contacts on the Edit Employee page.</summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class ContactsController : DnnApiController
    {
        private readonly EmergencyContactController _contacts = new EmergencyContactController();

        [HttpGet]
        [ActionName("ForEmployee")]
        public HttpResponseMessage ForEmployee(int employeeId)
        {
            try { return Request.CreateResponse(HttpStatusCode.OK, _contacts.GetForEmployee(employeeId)); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var item = _contacts.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(EmergencyContactInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (item.EmployeeId <= 0) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "EmployeeId required");
            try
            {
                _contacts.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, EmergencyContactInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                item.ContactId = id;
                _contacts.Update(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try { _contacts.Delete(id); return Request.CreateResponse(HttpStatusCode.NoContent); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
