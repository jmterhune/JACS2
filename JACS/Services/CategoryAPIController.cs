using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
                string searchTerm = !query.ContainsKey("q") ? "" : query["q"].ToString();

                var ctl = new CategoryController();
                categories = ctl.GetCategoryDropDownItems(searchTerm);
                return Request.CreateResponse(new CategoryListItemResult {data=categories,error=null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CategorySearchResult { data = null, error = ex.Message });
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
            int sortIndex = 2; // Default sort index
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            string sortColumn = GetSortColumn(sortIndex);
            string sortDirection = "asc"; // Default sort direction

            // Check if order parameters exist
            if (query.ContainsKey("order[0].column"))
            {
                Int32.TryParse(query["order[0].column"], out sortIndex);
                sortColumn = GetSortColumn(sortIndex);
            }
            if (query.ContainsKey("order[0].dir"))
            {
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new CategoryController();
                filteredCount = ctl.GetCategoriesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                categories = ctl.GetCategoriesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(category => new CategoryViewModel(category)).ToList();
                return Request.CreateResponse(new CategorySearchResult { data = categories, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CategorySearchResult { data = categories, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteCategory(long p1)
        {
            try
            {
                var ctl = new CategoryController();
                ctl.DeleteCategory(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetCategory(long p1)
        {
            try
            {
                var ctl = new CategoryController();
                Category category = ctl.GetCategory(p1);
                return Request.CreateResponse(new CategoryResult { data = category, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CategoryResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCategory(JObject p1)
        {
            try
            {
                var ctl = new CategoryController();
                var category = p1.ToObject<Category>();
                category.created_at = DateTime.Now;
                category.updated_at = DateTime.Now;
                ctl.CreateCategory(category);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCategory(JObject p1)
        {
            try
            {
                var ctl = new CategoryController();
                var category = p1.ToObject<Category>();
                category.updated_at = DateTime.Now;
                ctl.UpdateCategory(category);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
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
            string fieldName = "description";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "description";
                    break;
                default:
                    fieldName = "description";
                    break;
            }
            return fieldName;
        }
    }
}