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
    /// Read + delete endpoints for the Requests admin list. Requests are created
    /// from the public View; this list only lists, shows detail, and deletes.
    /// </summary>
    [ExpertWitnessAdminAuthorize]
    [ValidateAntiForgeryToken]
    public class RequestsController : DnnApiController
    {
        private readonly RequestController _ctrl = new RequestController();

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try
            {
                var items = _ctrl.GetRequestListItems().Select(x => new RequestListDto
                {
                    RequestID = x.RequestID,
                    CaseNumber = x.CaseNumber,
                    LocationName = x.LocationName,
                    TemplateName = x.TemplateName,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate
                });
                return Request.CreateResponse(HttpStatusCode.OK, items);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var req = _ctrl.GetRequestListItem(id);
                if (req == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                var tCtl = new TemplateController();
                var eCtl = new ExpertController();

                var requirements = tCtl.GetTemplateSequences(req.TemplateID)
                    .OrderBy(s => s.Sequence)
                    .Select(s => new RequestRequirementDto
                    {
                        Sequence = s.Sequence,
                        NumberRequired = s.NumberRequired,
                        Types = string.Join(" or ", tCtl.GetTemplateTypeTypesBySequence(s.TemplateID, s.Sequence).Select(t => t.TypeName))
                    })
                    .ToList();

                var experts = eCtl.GetExpertRequestListItems(id)
                    .OrderBy(x => x.Sequence)
                    .Select(x => new RequestExpertDto { ExpertID = x.ExpertID, Sequence = x.Sequence, Description = x.Description })
                    .ToList();

                var detail = new RequestDetailDto
                {
                    RequestID = req.RequestID,
                    CaseNumber = req.CaseNumber,
                    LocationName = req.LocationName,
                    TemplateName = req.TemplateName,
                    CreatedBy = req.CreatedBy,
                    CreatedDate = req.CreatedDate,
                    Requirements = requirements,
                    Experts = experts
                };
                return Request.CreateResponse(HttpStatusCode.OK, detail);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try { _ctrl.DeleteRequest(id); return Request.CreateResponse(HttpStatusCode.NoContent); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
