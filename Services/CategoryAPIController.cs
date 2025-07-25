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
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CategoryAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCategoryDropDownItems()
        {
            List<KeyValuePair<long, string>> categories = new List<KeyValuePair<long, string>>();
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                string searchTerm = query.ContainsKey("q") ? query["q"].ToString() : "";

                var ctl = new CategoryController();
                categories = ctl.GetCategoryDropDownItems(searchTerm);
                return Request.CreateResponse(HttpStatusCode.OK, new CategoryListItemResult { data = categories, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CategoryListItemResult { data = categories, error = $"Failed to retrieve category dropdown items: {ex.Message}" });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetCategories(int p1)
        {
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = query.ContainsKey("searchText") ? query["searchText"].ToString() : "";
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "description"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new CategoryController();
                filteredCount = ctl.GetCategoriesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                var categoriesPaged = ctl.GetCategoriesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection);
                if (categoriesPaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new CategorySearchResult
                    {
                        data = categories,
                        draw = draw,
                        recordsFiltered = filteredCount,
                        recordsTotal = recordCount,
                        error = "No categories found."
                    });
                }
                categories = categoriesPaged.Select(category => new CategoryViewModel(category)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new CategorySearchResult
                {
                    data = categories,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CategorySearchResult
                {
                    data = categories,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = $"Failed to retrieve categories: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteCategory(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid category ID." });
                }
                var ctl = new CategoryController();
                var category = ctl.GetCategory(p1);
                if (category == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Category not found." });
                }
                ctl.DeleteCategory(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetCategory(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new CategoryResult { data = null, error = "Invalid category ID." });
                }
                var ctl = new CategoryController();
                var category = ctl.GetCategory(p1);
                if (category == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CategoryResult { data = null, error = "Category not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new CategoryResult { data = category, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CategoryResult { data = null, error = $"Failed to retrieve category: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCategory(JObject p1)
        {
            try
            {
                var category = p1.ToObject<Category>();
                if (string.IsNullOrWhiteSpace(category.description))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Category description is required." });
                }
                var ctl = new CategoryController();
                category.created_at = DateTime.Now;
                category.updated_at = DateTime.Now;
                ctl.CreateCategory(category);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Entity created/updated/deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to create category: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCategory(JObject p1)
        {
            try
            {
                var category = p1.ToObject<Category>();
                if (category.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Category ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(category.description))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Category description is required." });
                }
                var ctl = new CategoryController();
                var existingCategory = ctl.GetCategory(category.id);
                if (existingCategory == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Category not found." });
                }
                category.updated_at = DateTime.Now;
                ctl.UpdateCategory(category);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Entity created/updated/deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to update category: {ex.Message}" });
            }
        }

        internal class CategorySearchResult
        {
            public List<CategoryViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CategoryResult
        {
            public Category data { get; set; }
            public string error { get; set; }
        }

        internal class CategoryListItemResult
        {
            public List<KeyValuePair<long, string>> data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "description";
                default:
                    return "description";
            }
        }
    }
}