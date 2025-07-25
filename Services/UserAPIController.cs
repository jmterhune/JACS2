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
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "name"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new jacs.Components.UserController();
                filteredCount = ctl.GetUsersCount(searchTerm, 0);
                if (p1 == 0) { recordCount = filteredCount; }
                users = ctl.GetUsersPaged(searchTerm, 0, recordOffset, pageSize, sortColumn, sortDirection).Select(user => new UserViewModel(user)).ToList();
                return Request.CreateResponse(new UserSearchResult { data = users, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new UserSearchResult { data = users, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetUsers(int p1, int p2)
        {
            List<UserViewModel> users = new List<UserViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string searchTerm = query.ContainsKey("searchText") ? query["searchText"].ToString() : "";
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "name"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new jacs.Components.UserController();
                filteredCount = ctl.GetUsersCount(searchTerm, p2);
                if (p1 == 0) { recordCount = filteredCount; }
                users = ctl.GetUsersPaged(searchTerm, p2, recordOffset, pageSize, sortColumn, sortDirection).Select(user => new UserViewModel(user)).ToList();
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
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetUser(long p1)
        {
            try
            {
                var ctl = new jacs.Components.UserController();
                User user = ctl.GetUser(p1);
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new UserResult { data = null, error = "User not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new UserResult { data = user, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new UserResult { data = null, error = ex.Message });
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
                var distinctUsers = users
                    .Where(u => !existingUsers.Contains(u.UserID))
                    .OrderBy(u => u.UserID)
                    .Select(u => new UserViewModel
                    {
                        id = u.UserID,
                        name = u.DisplayName,
                        email = u.Email
                    }).Distinct().ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new UserListResult { data = distinctUsers, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
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
                if (user == null || string.IsNullOrWhiteSpace(user.name) || string.IsNullOrWhiteSpace(user.email))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name and Email are required." });
                }
                var newUser = new jacs.Components.User
                {
                    id = user.id,
                    name = user.name,
                    email = user.email,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                ctl.CreateUser(newUser);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "User created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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
                if (user.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "User ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(user.name) || string.IsNullOrWhiteSpace(user.email))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name and Email are required." });
                }
                var existingUser = ctl.GetUser(user.id);
                if (existingUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "User not found." });
                }
                user.updated_at = DateTime.Now;
                ctl.UpdateUser(user);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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