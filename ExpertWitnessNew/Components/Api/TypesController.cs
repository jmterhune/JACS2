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
using EwType = tjc.Modules.ExpertWitness.Components.Type;

namespace tjc.Modules.ExpertWitness.Components.Api
{
    /// <summary>REST endpoints for the Expert Types admin list.</summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [ValidateAntiForgeryToken]
    public class TypesController : DnnApiController
    {
        private readonly TypeController _ctrl = new TypeController();

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try
            {
                var items = _ctrl.GetTypes()
                    .OrderBy(x => x.TypeName)
                    .Select(x => new TypeDto { TypeID = x.TypeID, TypeName = x.TypeName });
                return Request.CreateResponse(HttpStatusCode.OK, items);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var x = _ctrl.GetType(id);
                if (x == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, new TypeDto { TypeID = x.TypeID, TypeName = x.TypeName });
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(TypeDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.TypeName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Type name is required.");
            try
            {
                var entity = new EwType
                {
                    TypeName = item.TypeName.Trim(),
                    CreatedBy = UserInfo.Username,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = UserInfo.Username,
                    ModifiedDate = DateTime.Now
                };
                _ctrl.CreateType(entity);
                item.TypeID = entity.TypeID;
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, TypeDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.TypeName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Type name is required.");
            try
            {
                var entity = _ctrl.GetType(id);
                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                entity.TypeName = item.TypeName.Trim();
                entity.ModifiedBy = UserInfo.Username;
                entity.ModifiedDate = DateTime.Now;
                _ctrl.UpdateType(entity);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try { _ctrl.DeleteType(id); return Request.CreateResponse(HttpStatusCode.NoContent); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
