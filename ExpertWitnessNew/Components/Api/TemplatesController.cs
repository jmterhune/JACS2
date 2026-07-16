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
    /// <summary>
    /// REST endpoints for the Evaluation Types (templates) admin list. Each template
    /// has a set of requirements (sequence + number-required + the qualifying types).
    /// </summary>
    [ExpertWitnessAdminAuthorize]
    [ValidateAntiForgeryToken]
    public class TemplatesController : DnnApiController
    {
        private readonly TemplateController _ctrl = new TemplateController();

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try
            {
                var items = _ctrl.GetTemplates().OrderBy(x => x.TemplateName).Select(x => new TemplateListDto
                {
                    TemplateID = x.TemplateID,
                    TemplateName = x.TemplateName,
                    ExcludeSouthCounty = x.ExcludeSouthCounty,
                    TypesRequired = BuildTypesRequired(x.TemplateID)
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
                var x = _ctrl.GetTemplate(id);
                if (x == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                var requirements = _ctrl.GetTemplateSequences(id).OrderBy(s => s.Sequence).Select(s => new TemplateRequirementDto
                {
                    Sequence = s.Sequence,
                    NumberRequired = s.NumberRequired,
                    TypeIDs = _ctrl.GetTemplateTypeTypesBySequence(id, s.Sequence).Select(t => t.TypeID).ToList()
                }).ToList();
                var dto = new TemplateEditDto { TemplateID = x.TemplateID, TemplateName = x.TemplateName, ExcludeSouthCounty = x.ExcludeSouthCounty, Requirements = requirements };
                return Request.CreateResponse(HttpStatusCode.OK, dto);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(TemplateEditDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.TemplateName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Evaluation type name is required.");
            try
            {
                var entity = new Template
                {
                    TemplateName = item.TemplateName.Trim(),
                    ExcludeSouthCounty = item.ExcludeSouthCounty,
                    CreatedBy = UserInfo.Username,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = UserInfo.Username,
                    ModifiedDate = DateTime.Now
                };
                _ctrl.CreateTemplate(entity);
                SaveRequirements(entity.TemplateID, item);
                item.TemplateID = entity.TemplateID;
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, TemplateEditDto item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.TemplateName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Evaluation type name is required.");
            try
            {
                var entity = _ctrl.GetTemplate(id);
                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                entity.TemplateName = item.TemplateName.Trim();
                entity.ExcludeSouthCounty = item.ExcludeSouthCounty;
                entity.ModifiedBy = UserInfo.Username;
                entity.ModifiedDate = DateTime.Now;
                // Replace requirements, then update the template row.
                _ctrl.DeleteTemplateTypes(id);
                _ctrl.DeleteTemplateSequences(id);
                _ctrl.UpdateTemplate(entity);
                SaveRequirements(id, item);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                _ctrl.DeleteTemplateTypes(id);
                _ctrl.DeleteTemplateSequences(id);
                _ctrl.DeleteTemplate(id);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        /// <summary>Flip the ExcludeSouthCounty flag for a single evaluation type (inline list toggle).</summary>
        [HttpPost]
        [ActionName("ToggleSouthCounty")]
        public HttpResponseMessage ToggleSouthCounty(int id)
        {
            try
            {
                var entity = _ctrl.GetTemplate(id);
                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                entity.ExcludeSouthCounty = !entity.ExcludeSouthCounty;
                entity.ModifiedBy = UserInfo.Username;
                entity.ModifiedDate = DateTime.Now;
                _ctrl.UpdateTemplate(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { entity.TemplateID, entity.ExcludeSouthCounty });
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        private void SaveRequirements(int templateId, TemplateEditDto item)
        {
            int seq = 0;
            foreach (var req in item.Requirements ?? new List<TemplateRequirementDto>())
            {
                seq++; // re-index 1..N regardless of incoming Sequence
                _ctrl.CreateTemplateSequence(templateId, seq, req.NumberRequired);
                foreach (var typeId in req.TypeIDs ?? new List<int>())
                    _ctrl.CreatTemplateType(templateId, typeId, seq);
            }
        }

        private string BuildTypesRequired(int templateId)
        {
            // Mirrors the Template.TypesRequired display: "TypeA, TypeB(2) - TypeC(1)".
            var parts = new List<string>();
            foreach (var s in _ctrl.GetTemplateSequences(templateId).OrderBy(x => x.Sequence))
            {
                var names = string.Join(", ", _ctrl.GetTemplateTypeTypesBySequence(templateId, s.Sequence).Select(t => t.TypeName));
                parts.Add(names + "(" + s.NumberRequired + ")");
            }
            return string.Join(" - ", parts);
        }
    }
}
