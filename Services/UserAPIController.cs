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
    public class UserAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetUsers(int p1)
        {
            List<UserViewModel> users = new List<UserViewModel>();
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
                var ctl = new jacs.Components.UserController();
                filteredCount = ctl.GetUsersCount(searchTerm,0);
                if (p1 == 0) { recordCount = filteredCount; }
                users = ctl.GetUsersPaged(searchTerm,0, recordOffset, pageSize, sortColumn, sortDirection).Select(user => new UserViewModel(user)).ToList();
                return Request.CreateResponse(new UserSearchResult { data = users, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new UserSearchResult { data = users, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetUsers(int p1,int p2)
        {
            List<UserViewModel> users = new List<UserViewModel>();
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
                var ctl = new jacs.Components.UserController();
                filteredCount = ctl.GetUsersCount(searchTerm,p2);
                if (p1 == 0) { recordCount = filteredCount; }
                users = ctl.GetUsersPaged(searchTerm,p2, recordOffset, pageSize, sortColumn, sortDirection).Select(user => new UserViewModel(user)).ToList();
                return Request.CreateResponse(new UserSearchResult { data = users, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new UserSearchResult { data = users, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteUser(long p1)
        {
            try
            {
                var ctl = new jacs.Components.UserController();
                ctl.DeleteUser(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetUser(long p1)
        {
            try
            {
                var ctl = new jacs.Components.UserController();
                User user = ctl.GetUser(p1);
                return Request.CreateResponse(new UserResult { data = user, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new UserResult { data = null, error = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetUsersByRole(string p1)
        {
            try
            {
                int portalId = PortalSettings.PortalId;
                var roleList = p1.Split(',').Select(r => r.Trim()).ToList();
                var users = new List<UserInfo>();
                var ctl = new jacs.Components.UserController();
                var existingUsers = ctl.GetUsers().Select(u => u.id).ToList();
                foreach (var role in roleList)
                {
                    var roleUsers = DotNetNuke.Security.Roles.RoleController.Instance.GetUsersByRole(portalId, role);
                    users.AddRange(roleUsers);
                }
                var distinctUsers = users.Where( u=>!existingUsers.Contains(u.UserID)).OrderBy(u => u.UserID).Select(u => new UserViewModel
                {
                    id = u.UserID,
                    name = u.DisplayName,
                    email = u.Email
                }).Distinct().ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new UserListResult { data = distinctUsers, error = null });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new UserListResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateUser(JObject p1)
        {
            try
            {
                var ctl = new jacs.Components.UserController();
                var user = p1.ToObject<UserViewModel>();
                if (user != null)
                {
                    ctl.CreateUser(new jacs.Components.User { id = user.id, name = user.name, email = user.email,  created_at = DateTime.Now, updated_at = DateTime.Now });
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK);
                }
                else
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound,"Unable to Create User");
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateUser(JObject p1)
        {
            try
            {
                var ctl = new jacs.Components.UserController();
                var user = p1.ToObject<User>();
                ctl.UpdateUser(user);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class UserSearchResult
        {
            public List<UserViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class UserResult
        {
            public User data { get; set; }
            public string error { get; set; }
        }
        internal class UserListResult
        {
            public List<UserViewModel> data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            string fieldName = "name";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "name";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}