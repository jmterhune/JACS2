/*
' Copyright (c) 2019  jud12
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    /// <summary>
    /// View-only list of incidents for the intranet site. Permissions are controlled via
    /// DNN; this module never accepts input or sends mail.
    /// </summary>
    public partial class View : ThreatReportModuleBase
    {
        private readonly IHostSettings _hostSettings;

        public View()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    // If the request carries an "id" query parameter (e.g. a link from the
                    // internet site's email of the form [IntranetAppUrl]/id/N), forward to
                    // the detail control instead of rendering the list. This is what makes
                    // bare URLs like /Judiciary/Threat-Report/id/164 land on the incident view.
                    string idParam = Request.QueryString["id"];
                    int incidentId;
                    if (int.TryParse(idParam, out incidentId) && incidentId > 0)
                    {
                        Response.Redirect(EditUrl("id", incidentId.ToString(), "incident"));
                        return;
                    }

                    // Anyone who can view the list can add a threat.
                    lnkEdit.NavigateUrl = EditUrl();
                    lnkEdit.Visible = true;
                    RefreshNotifyButton();

                    IncidentController ctl = new IncidentController(_hostSettings);
                    rptIncidentList.DataSource = ctl.GetIncidents().Where(x => x.Location != null);
                    rptIncidentList.DataBind();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private string NotificationRole
        {
            get
            {
                if (Settings.Contains("ViewerRole") && !string.IsNullOrWhiteSpace(Settings["ViewerRole"].ToString()))
                {
                    return Settings["ViewerRole"].ToString();
                }
                return IncidentNotificationController.DefaultRoleName;
            }
        }

        // Receiving notifications means being in the role on the jud12 site (where the external form
        // sends from) and/or the local role of the same name; both are managed together here.
        private bool IsReceiving(IncidentNotificationController ctl, SubscriptionInfo jud12)
        {
            return jud12.State == SubscriptionState.Subscribed
                || ctl.IsInLocalRole(PortalId, UserId, NotificationRole);
        }

        private void RefreshNotifyButton()
        {
            if (UserId <= 0) return;
            var ctl = new IncidentNotificationController(_hostSettings);
            SubscriptionInfo jud12 = ctl.GetSubscription(UserInfo.Username, UserInfo.Email, NotificationRole);
            cmdNotify.Text = IsReceiving(ctl, jud12) ? "Stop Receiving Incident Report Notifications" : "Receive Incident Report Notifications";
            cmdNotify.Visible = true;
        }

        protected void cmdNotify_Click(object sender, EventArgs e)
        {
            try
            {
                var ctl = new IncidentNotificationController(_hostSettings);
                string role = NotificationRole;
                SubscriptionInfo jud12 = ctl.GetSubscription(UserInfo.Username, UserInfo.Email, role);
                var message = DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess;
                string text;

                if (IsReceiving(ctl, jud12))
                {
                    if (jud12.State == SubscriptionState.Subscribed) ctl.Unsubscribe(jud12);
                    ctl.SetLocalRole(PortalId, UserId, role, false);
                    text = "You will no longer receive incident report notifications.";
                }
                else
                {
                    bool addedJud12 = false;
                    if (jud12.State == SubscriptionState.NotSubscribed)
                    {
                        ctl.Subscribe(jud12, UserId);
                        addedJud12 = true;
                    }
                    bool addedLocal = ctl.SetLocalRole(PortalId, UserId, role, true);
                    if (addedJud12 || addedLocal)
                    {
                        text = "You will now receive incident report notifications.";
                        if (!addedJud12 && jud12.State == SubscriptionState.Ambiguous)
                        {
                            text += " Your email matches more than one account on the jud12 site, so only the intranet role was updated.";
                        }
                    }
                    else
                    {
                        message = DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning;
                        text = "Could not add you to \"" + role + "\": there is no matching role or account.";
                    }
                }

                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, text, message);
                RefreshNotifyButton();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
