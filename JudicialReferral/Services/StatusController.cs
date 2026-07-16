/*
 * StatusController — WebAPI endpoint for in-place status updates from the
 * View.ascx list. Used by the Court Counsel role to change a referral's
 * Status without a full page postback.
 */

using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Web.Api;
using tjc.Modules.JudicialReferral.Components.Controllers;
using tjc.Modules.JudicialReferral.Components.Models;

namespace tjc.Modules.JudicialReferral.Services
{
    [DnnAuthorize]
    public class StatusController : DnnApiController
    {
        public class UpdateRequest
        {
            public int ReferralId { get; set; }
            public int Status { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Update(UpdateRequest req)
        {
            if (req == null || req.ReferralId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Missing referralId." });

            // Restrict to Court Counsel Admin role (or superusers). Role name is
            // configured per-module via Settings["CounselAdminRole"]; fall back to
            // "Court Counsel Admin".
            string counselAdminRole = "Court Counsel Admin";
            if (ActiveModule != null && ActiveModule.ModuleSettings != null
                && ActiveModule.ModuleSettings.Contains("CounselAdminRole"))
            {
                counselAdminRole = ActiveModule.ModuleSettings["CounselAdminRole"].ToString();
            }

            if (!UserInfo.IsSuperUser && !UserInfo.IsInRole(counselAdminRole))
                return Request.CreateResponse(HttpStatusCode.Forbidden, new { error = "Forbidden." });

            // Accept only the four known status codes.
            if (req.Status != (int)Statuses.NewReferral
                && req.Status != (int)Statuses.ReferredToCounsel
                && req.Status != (int)Statuses.RetainedByJudge
                && req.Status != (int)Statuses.Completed)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Unknown status code." });
            }

            var ctl = new JudgeReferralController();
            ctl.UpdateStatus(req.ReferralId, req.Status);

            // Mirror the Review view's behavior: when a referral transitions to
            // "Referred to Court Counsel", notify Counsel by email. Failures here
            // do not abort the status update — the status is already saved.
            if (req.Status == (int)Statuses.ReferredToCounsel)
            {
                try
                {
                    var objReferral = ctl.GetReferral(req.ReferralId);
                    if (objReferral != null)
                    {
                        string counselEmail = GetCourtCounselEmail();
                        int tabId = ActiveModule != null ? ActiveModule.TabID : PortalSettings.ActiveTab.TabID;
                        int moduleId = ActiveModule != null ? ActiveModule.ModuleID : 0;
                        JudgeReferralController.SendToCounsel(objReferral, PortalSettings.PortalId, tabId, moduleId, counselEmail);
                    }
                }
                catch (System.Exception ex)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                referralId = req.ReferralId,
                status = req.Status,
                statusName = GetStatusName(req.Status)
            });
        }

        private string GetCourtCounselEmail()
        {
            if (ActiveModule != null && ActiveModule.ModuleSettings != null
                && ActiveModule.ModuleSettings.Contains("CourtCounselEmail"))
            {
                return ActiveModule.ModuleSettings["CourtCounselEmail"].ToString();
            }
            return string.Empty;
        }

        private static string GetStatusName(int status)
        {
            switch (status)
            {
                case 1: return "New";
                case 3: return "Referred to Court Counsel";
                case 4: return "Retained by Judge";
                case 5: return "Completed";
                default: return string.Empty;
            }
        }
    }
}
