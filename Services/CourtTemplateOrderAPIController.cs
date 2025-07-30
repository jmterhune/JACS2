using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CourtTemplateOrderAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage Index(long courtId)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                string searchTerm = query.ContainsKey("searchText") ? query["searchText"] : string.Empty;
                Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
                Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
                Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);
                string sortColumn = "date";
                string sortDirection = "asc";

                if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
                {
                    Int32.TryParse(query["order[0].column"], out int sortIndex);
                    sortColumn = GetSortColumn(sortIndex);
                    sortDirection = query["order[0].dir"];
                }

                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }

                var templateOrderCtl = new CourtTemplateOrderController();
                var filteredCount = templateOrderCtl.GetCourtTemplateOrdersCount(courtId, searchTerm);
                var totalCount = filteredCount;
                var templateOrders = templateOrderCtl.GetCourtTemplateOrdersPaged(courtId, searchTerm, recordOffset, pageSize, sortColumn, sortDirection).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    draw = draw,
                    recordsTotal = totalCount,
                    recordsFiltered = filteredCount,
                    data = templateOrders.Select(to => new
                    {
                        id = to.id,
                        court_id = to.court_id,
                        template_id = to.template_id,
                        date = to.date.Value.ToString("yyyy-MM-dd"),
                        auto = to.auto,
                        created_at = to.created_at,
                        updated_at = to.updated_at
                    }),
                    error = (string)null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage Show(long id)
        {
            try
            {
                var templateOrderCtl = new CourtTemplateOrderController();
                var templateOrder = templateOrderCtl.GetCourtTemplateOrder(id);
                if (templateOrder == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court template order not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    data = new
                    {
                        id = templateOrder.id,
                        court_id = templateOrder.court_id,
                        template_id = templateOrder.template_id,
                        date = templateOrder.date.HasValue?templateOrder.date.Value.ToString("yyyy-MM-dd"):null,
                        auto = templateOrder.auto,
                        created_at = templateOrder.created_at,
                        updated_at = templateOrder.updated_at
                    },
                    error = (string)null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Store(JObject data)
        {
            try
            {
                var templateOrder = data.ToObject<CourtTemplateOrder>();
                if (templateOrder.court_id <= 0 || templateOrder.template_id <= 0 || templateOrder.date == default)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID, template ID, and date are required." });
                }
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(templateOrder.court_id);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var templateCtl = new CourtTemplateController();
                var template = templateCtl.GetCourtTemplate(templateOrder.template_id.Value);
                if (template == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template not found." });
                }
                templateOrder.created_at = DateTime.Now;
                templateOrder.updated_at = DateTime.Now;
                var templateOrderCtl = new CourtTemplateOrderController();
                templateOrderCtl.CreateCourtTemplateOrder(templateOrder);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Court template order created successfully",
                    data = new
                    {
                        id = templateOrder.id,
                        court_id = templateOrder.court_id,
                        template_id = templateOrder.template_id,
                        date = templateOrder.date.Value.ToString("yyyy-MM-dd"),
                        auto = templateOrder.auto,
                        created_at = templateOrder.created_at,
                        updated_at = templateOrder.updated_at
                    }
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Update(long id, JObject data)
        {
            try
            {
                var templateOrder = data.ToObject<CourtTemplateOrder>();
                if (id <= 0 || templateOrder.id != id)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Valid court template order ID is required." });
                }
                if (templateOrder.court_id <= 0 || templateOrder.template_id <= 0 || templateOrder.date == default)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID, template ID, and date are required." });
                }
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(templateOrder.court_id);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var templateCtl = new CourtTemplateController();
                var template = templateCtl.GetCourtTemplate(templateOrder.template_id.Value);
                if (template == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template not found." });
                }
                var templateOrderCtl = new CourtTemplateOrderController();
                var existingTemplateOrder = templateOrderCtl.GetCourtTemplateOrder(id);
                if (existingTemplateOrder == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court template order not found." });
                }
                templateOrder.updated_at = DateTime.Now;
                templateOrderCtl.UpdateCourtTemplateOrder(templateOrder);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Court template order updated successfully",
                    data = new
                    {
                        id = templateOrder.id,
                        court_id = templateOrder.court_id,
                        template_id = templateOrder.template_id,
                        date = templateOrder.date.Value.ToString("yyyy-MM-dd"),
                        auto = templateOrder.auto,
                        created_at = templateOrder.created_at,
                        updated_at = templateOrder.updated_at
                    }
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage Destroy(long id)
        {
            try
            {
                var templateOrderCtl = new CourtTemplateOrderController();
                var templateOrder = templateOrderCtl.GetCourtTemplateOrder(id);
                if (templateOrder == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court template order not found." });
                }
                templateOrderCtl.DeleteCourtTemplateOrder(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court template order deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return "id";
                case 1:
                    return "court_id";
                case 2:
                    return "template_id";
                case 3:
                    return "date";
                case 4:
                    return "auto";
                default:
                    return "date";
            }
        }
    }
}