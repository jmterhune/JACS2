/*
' Copyright (c) 2024  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.ExpertWitness.Components;

namespace tjc.Modules.ExpertWitness.Components.Api
{
    /// <summary>REST endpoints for the Experts admin list (with Location / Type / Evaluation associations).</summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [ValidateAntiForgeryToken]
    public class ExpertsController : DnnApiController
    {
        private readonly ExpertController _ctrl = new ExpertController();

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try
            {
                var items = _ctrl.GetExperts().OrderBy(x => x.Description).Select(x => new ExpertListDto
                {
                    ExpertID = x.ExpertID,
                    Description = x.Description,
                    ContractEnds = x.ContractEnds,
                    Comments = x.Comments,
                    TypeDisplay = string.Join(", ", _ctrl.GetExpertTypeTypes(x.ExpertID).Select(t => t.TypeName)),
                    LocationDisplay = string.Join(", ", _ctrl.GetExpertLocationLocations(x.ExpertID).Select(l => l.LocationName))
                }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, items);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var x = _ctrl.GetExpert(id);
                if (x == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                var dto = new ExpertEditDto
                {
                    ExpertID = x.ExpertID,
                    Description = x.Description,
                    ContractEnds = x.ContractEnds,
                    Comments = x.Comments,
                    LocationIDs = _ctrl.GetExpertLocationLocations(id).Select(l => l.LocationID).ToList(),
                    TypeIDs = _ctrl.GetExpertTypeTypes(id).Select(t => t.TypeID).ToList(),
                    TemplateIDs = _ctrl.GetExpertTemplateTemplates(id).Select(t => t.TemplateID).ToList()
                };
                return Request.CreateResponse(HttpStatusCode.OK, dto);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(ExpertEditDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Description))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Expert name is required.");
            try
            {
                var entity = new Expert
                {
                    Description = item.Description.Trim(),
                    ContractEnds = item.ContractEnds,
                    Comments = item.Comments,
                    CreatedBy = UserInfo.Username,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = UserInfo.Username,
                    ModifiedDate = DateTime.Now
                };
                _ctrl.CreateExpert(entity);
                SaveAssociations(entity.ExpertID, item);
                item.ExpertID = entity.ExpertID;
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, ExpertEditDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Description))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Expert name is required.");
            try
            {
                var entity = _ctrl.GetExpert(id);
                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                entity.Description = item.Description.Trim();
                entity.ContractEnds = item.ContractEnds;
                entity.Comments = item.Comments;
                entity.ModifiedBy = UserInfo.Username;
                entity.ModifiedDate = DateTime.Now;
                _ctrl.UpdateExpert(entity);
                // Replace associations.
                _ctrl.DeleteExpertLocations(id);
                _ctrl.DeleteExpertTypes(id);
                _ctrl.DeleteExpertTemplates(id);
                SaveAssociations(id, item);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                _ctrl.DeleteExpertLocations(id);
                _ctrl.DeleteExpertTypes(id);
                _ctrl.DeleteExpertTemplates(id);
                _ctrl.DeleteExpert(id);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        private void SaveAssociations(int expertId, ExpertEditDto item)
        {
            foreach (var locationId in item.LocationIDs ?? new List<int>())
                _ctrl.CreateExpertLocation(expertId, locationId);
            foreach (var typeId in item.TypeIDs ?? new List<int>())
                _ctrl.CreateExpertType(expertId, typeId);
            foreach (var templateId in item.TemplateIDs ?? new List<int>())
                _ctrl.CreateExpertTemplate(expertId, templateId);
        }
    }
}
