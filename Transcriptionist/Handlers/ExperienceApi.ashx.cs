using System;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using Microsoft.Extensions.DependencyInjection;
using tjc.Modules.Transcriptionist.Components;

namespace tjc.Modules.Transcriptionist.Handlers
{
    // JSON API for the experience records on ApplicationEdit.ascx, so they can be managed without postbacks.
    // Only staff with Admin or Manage permission on a Transcriptionist module may call it.
    public class ExperienceApi : IHttpHandler
    {
        // The *ByUserID columns are shared with the jud12 site; 0 marks "changed on the intranet".
        private const int IntranetUserId = 0;

        private IHostSettings _hostSettings;

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            _hostSettings = context.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
            context.Response.ContentType = "application/json";
            var serializer = new JavaScriptSerializer();

            try
            {
                var user = UserController.Instance.GetCurrentUserInfo();
                if (user == null || user.UserID <= 0)
                {
                    WriteJson(context, serializer, new { success = false, error = "Not authenticated." });
                    return;
                }

                if (!HasStaffAccess(context, user))
                {
                    WriteJson(context, serializer, new { success = false, error = "You do not have permission to manage applications." });
                    return;
                }

                string action = context.Request["action"];
                bool changes = action == "saveExperience" || action == "deleteExperience";

                // A page on another site cannot add this header to a request without the browser asking this
                // site first, so requiring it (with POST) stops cross-site forged changes.
                if (changes && (context.Request.HttpMethod != "POST" || context.Request.Headers["X-Requested-With"] != "XMLHttpRequest"))
                {
                    WriteJson(context, serializer, new { success = false, error = "Invalid request." });
                    return;
                }

                long tid;
                if (!long.TryParse(context.Request["tid"], out tid) || tid <= 0
                    || new ApplicationController(_hostSettings).GetApplication(tid) == null)
                {
                    WriteJson(context, serializer, new { success = false, error = "Application not found." });
                    return;
                }

                switch (action)
                {
                    case "listExperience":
                        ListExperience(context, serializer, tid);
                        break;
                    case "saveExperience":
                        SaveExperience(context, serializer, tid);
                        break;
                    case "deleteExperience":
                        DeleteExperience(context, serializer, tid);
                        break;
                    default:
                        WriteJson(context, serializer, new { success = false, error = "Unknown action." });
                        break;
                }
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                WriteJson(context, serializer, new { success = false, error = "The request could not be completed." });
            }
        }

        private static void WriteJson(HttpContext context, JavaScriptSerializer serializer, object data)
        {
            context.Response.Write(serializer.Serialize(data));
        }

        // The caller names the module instance it is on ("mid"/"tabid"); it must be a Transcriptionist module
        // on which the user has ADMIN or MANAGE permission.
        private static bool HasStaffAccess(HttpContext context, UserInfo user)
        {
            if (user.IsSuperUser) return true;

            int moduleId, tabId;
            if (!int.TryParse(context.Request["mid"], out moduleId) || moduleId <= 0) return false;
            if (!int.TryParse(context.Request["tabid"], out tabId) || tabId <= 0) return false;

            var module = ModuleController.Instance.GetModule(moduleId, tabId, false);
            if (module == null) return false;

            string folder = module.DesktopModule?.FolderName ?? "";
            if (!folder.EndsWith("Transcriptionist", StringComparison.OrdinalIgnoreCase)) return false;

            return ModulePermissionController.HasModulePermission(module.ModulePermissions, "ADMIN")
                || ModulePermissionController.HasModulePermission(module.ModulePermissions, "MANAGE");
        }

        private void ListExperience(HttpContext context, JavaScriptSerializer serializer, long tid)
        {
            var items = new ExperienceController(_hostSettings).GetByTranscriptionistId(tid)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new
                {
                    experienceId = x.ExperienceID,
                    courtName = x.CourtName,
                    startDate = x.StartDate.HasValue ? x.StartDate.Value.ToString("MM/dd/yyyy") : "",
                    endDate = x.EndDate.HasValue ? x.EndDate.Value.ToString("MM/dd/yyyy") : "",
                    proceedingTypes = x.ProceedingTypes
                })
                .ToList();

            WriteJson(context, serializer, new { success = true, items });
        }

        private void SaveExperience(HttpContext context, JavaScriptSerializer serializer, long tid)
        {
            string courtName = (context.Request["courtName"] ?? "").Trim();
            DateTime start, end;
            bool hasStart = DateTime.TryParse(context.Request["startDate"], out start);
            bool hasEnd = DateTime.TryParse(context.Request["endDate"], out end);

            if (string.IsNullOrEmpty(courtName) || !hasStart || !hasEnd)
            {
                WriteJson(context, serializer, new { success = false, error = "Court Name, Start Date, and End Date are required." });
                return;
            }
            if (courtName.Length > 50)
            {
                WriteJson(context, serializer, new { success = false, error = "Court Name can be at most 50 characters." });
                return;
            }

            long experienceId;
            long.TryParse(context.Request["experienceId"], out experienceId);

            var expCtl = new ExperienceController(_hostSettings);
            Experience exp = experienceId > 0 ? expCtl.GetExperience(experienceId) : new Experience();

            if (experienceId > 0 && (exp == null || exp.TranscriptionistID != tid))
            {
                WriteJson(context, serializer, new { success = false, error = "Experience record not found." });
                return;
            }

            string proceedingTypes = (context.Request["proceedingTypes"] ?? "").Trim();
            exp.TranscriptionistID = tid;
            exp.CourtName = courtName;
            exp.StartDate = start;
            exp.EndDate = end;
            exp.ProceedingTypes = proceedingTypes.Length > 4000 ? proceedingTypes.Substring(0, 4000) : proceedingTypes;

            if (experienceId > 0)
            {
                exp.LastModifiedDate = DateTime.UtcNow;
                exp.LastModifiedByUserID = IntranetUserId;
                expCtl.UpdateExperience(exp);
            }
            else
            {
                exp.CreatedDate = DateTime.UtcNow;
                exp.CreatedByUserID = IntranetUserId;
                expCtl.CreateExperience(exp);
            }

            WriteJson(context, serializer, new { success = true });
        }

        private void DeleteExperience(HttpContext context, JavaScriptSerializer serializer, long tid)
        {
            long experienceId;
            long.TryParse(context.Request["experienceId"], out experienceId);

            var expCtl = new ExperienceController(_hostSettings);
            var exp = experienceId > 0 ? expCtl.GetExperience(experienceId) : null;
            if (exp == null || exp.TranscriptionistID != tid)
            {
                WriteJson(context, serializer, new { success = false, error = "Experience record not found." });
                return;
            }

            expCtl.DeleteExperience(experienceId);
            WriteJson(context, serializer, new { success = true });
        }
    }
}
