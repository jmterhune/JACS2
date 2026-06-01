/*
' Copyright (c) 2024  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.ExpertWitness.Components;

namespace tjc.Modules.ExpertWitness.Components.Api
{
    /// <summary>REST endpoints for the Locations admin list.</summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [ValidateAntiForgeryToken]
    public class LocationsController : DnnApiController
    {
        private readonly LocationController _ctrl = new LocationController();

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try
            {
                var items = _ctrl.GetLocations()
                    .OrderBy(x => x.LocationName)
                    .Select(x => new LocationDto { LocationID = x.LocationID, LocationName = x.LocationName });
                return Request.CreateResponse(HttpStatusCode.OK, items);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var x = _ctrl.GetLocation(id);
                if (x == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, new LocationDto { LocationID = x.LocationID, LocationName = x.LocationName });
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(LocationDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.LocationName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Location name is required.");
            try
            {
                var entity = new Location
                {
                    LocationName = item.LocationName.Trim(),
                    CreatedBy = UserInfo.Username,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = UserInfo.Username,
                    ModifiedDate = DateTime.Now
                };
                _ctrl.CreateLocation(entity);
                item.LocationID = entity.LocationID;
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, LocationDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.LocationName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Location name is required.");
            try
            {
                var entity = _ctrl.GetLocation(id);
                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                entity.LocationName = item.LocationName.Trim();
                entity.ModifiedBy = UserInfo.Username;
                entity.ModifiedDate = DateTime.Now;
                _ctrl.UpdateLocation(entity);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try { _ctrl.DeleteLocation(id); return Request.CreateResponse(HttpStatusCode.NoContent); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
