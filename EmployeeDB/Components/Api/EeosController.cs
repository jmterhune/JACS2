using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>
    /// REST endpoints for the EEO List on the EEO Setup page.
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class EeosController : DnnApiController
    {
        private readonly EeoController _eeo = new EeoController();
        private readonly JobGroupController _jobGroups = new JobGroupController();

        /// <summary>EEO row plus the resolved job-group description so the JS
        /// list can render the category name without an extra round-trip.</summary>
        private static EeoInfo Stamp(EeoInfo row, IDictionary<int, string> lookup)
        {
            if (row == null) return null;
            if (row.JobGroupId.HasValue && lookup.TryGetValue(row.JobGroupId.Value, out var name))
                row.JobGroupName = name;
            return row;
        }

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try
            {
                var lookup = _jobGroups.GetAll().ToDictionary(jg => jg.JobGroupId, jg => jg.Description);
                var rows = _eeo.GetAll()
                    .OrderByDescending(r => r.Year)
                    .ThenBy(r => r.JobGroupId)
                    .Select(r => Stamp(r, lookup))
                    .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, rows);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(long id)
        {
            try
            {
                var item = _eeo.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                var lookup = _jobGroups.GetAll().ToDictionary(jg => jg.JobGroupId, jg => jg.Description);
                Stamp(item, lookup);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(EeoInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                _eeo.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(long id, EeoInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                item.EeoId = id;
                _eeo.Update(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(long id)
        {
            try { _eeo.Delete(id); return Request.CreateResponse(HttpStatusCode.NoContent); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
