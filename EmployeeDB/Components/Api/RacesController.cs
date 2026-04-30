using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>REST endpoints for the Race admin tab on EmployeeList.</summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class RacesController : DnnApiController
    {
        private readonly RaceController _ctrl = new RaceController();

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try { return Request.CreateResponse(HttpStatusCode.OK, _ctrl.GetAll().OrderBy(x => x.RaceCode)); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var item = _ctrl.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(RaceInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                _ctrl.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, RaceInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                item.RaceId = id;
                _ctrl.Update(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try { _ctrl.Delete(id); return Request.CreateResponse(HttpStatusCode.NoContent); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
