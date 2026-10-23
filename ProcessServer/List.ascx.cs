/*
' Copyright (c) 2021  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using tjc.Modules.ProcessServer.Components;

namespace tjc.Modules.ProcessServer
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from ProcessServerModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class List : ProcessServerModuleBase
    {
        private DateTime startDate = DateTime.Now.AddDays(-30);
        private DateTime endDate = DateTime.Now;
        private readonly INavigationManager _navigationManager;
        public List()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        #region Methods
        private void BindData()
        {
            var ctl = new ProcessServerController(_hostSettings);
           
            Int32.TryParse(drpYear.SelectedValue,out int year);
            rptProcessServers.DataSource = ctl.GetItemsByYear(year);
            rptProcessServers.DataBind();
        }
        private void BindYears()
        {
            var ctl = new ProcessServerController(_hostSettings);
            drpYear.DataSource=ctl.GetApplicationYears();
            drpYear.DataBind();
        }
        public string GetUserName(string userId)
        {
            Int32.TryParse(userId, out int userID);
            var user = UserController.Instance.GetUserById(PortalId, userID);
            if (user != null)
                return string.Format("<span  data-bs-toggle=\"tooltip\" title=\"{0}\">{1}</span>", user.Email,user.Username);
            return string.Empty;
        }
        #endregion

        #region Event Handlers     
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    if (!UserInfo.IsInRole(AdminRole))
                    {
                        Response.Redirect(_navigationManager.NavigateURL());
                    }

                    txtStartDate.Text = startDate.ToShortDateString();
                    txtEndDate.Text = endDate.ToShortDateString();
                    BindYears();
                    drpYear.SelectedIndex = 0;
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    BindData();
                    var ctl = new TransactionController(_hostSettings);
                    rptTransactions.DataSource = ctl.GetTransactionsByDateRange(startDate, endDate.AddDays(1));
                    rptTransactions.DataBind();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            DateTime.TryParse(txtStartDate.Text, out DateTime start);
            DateTime.TryParse(txtEndDate.Text, out DateTime end);
            startDate = start;
            endDate = end;
            var ctl = new TransactionController(_hostSettings);
            rptTransactions.DataSource = ctl.GetTransactionsByDateRange(startDate, endDate.AddDays(1));
            rptTransactions.DataBind();

        }
        protected void rptProcessServers_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            Components.ProcessServer ps = (Components.ProcessServer)e.Item.DataItem;
            int processServerId = Int32.Parse(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "setpaid")
            {
                var ctl = new ProcessServerController(_hostSettings);
                Components.ProcessServer p = ctl.GetItem(processServerId);
                if (p.PaymentReceivedDate.HasValue)
                {
                    p.PaymentReceivedDate = null;
                }
                else
                {
                    p.PaymentReceivedDate = DateTime.Now;
                }
                ctl.UpdateItem(p);
                BindData();
            }
            if (e.CommandName.ToLower() == "setrenewal")
            {
                var ctl = new ProcessServerController(_hostSettings);
                Components.ProcessServer p = ctl.GetItem(processServerId);
                bool renewed = true;
                if (p.IsRenewal) { renewed = false; }
                p.IsRenewal = renewed;
                ctl.UpdateItem(p);
                BindData();
            }

            if (e.CommandName.ToLower() == "setchcompleted")
            {
                var ctl = new ProcessServerController(_hostSettings);
                Components.ProcessServer p = ctl.GetItem(processServerId);
                if (!p.CHCompleted) { p.CriminalHistoryCompleted = DateTime.Now; } else { p.CriminalHistoryCompleted = null; }
                if (p.Status == applicationStatus.CriminalHistoryComplete)
                {
                    p.Status = applicationStatus.New;
                    p.CriminalHistoryCompleted = null;
                    p.ApprovedByCourtCounsel = null;
                    p.ApprovedByChiefJudge = null;
                }
                else
                {
                    p.Status = applicationStatus.CriminalHistoryComplete;
                }
                ctl.UpdateItem(p);
                SendCourtCounselNotification(p);
                BindData();
            }
            if (e.CommandName.ToLower() == "delete")
            {
                var ctl = new ProcessServerController(_hostSettings);
                ctl.DeleteItem(processServerId);
                BindData();
            }
        }

        private void SendCourtCounselNotification(Components.ProcessServer p)
        {

            string subject = "Review Process Server Application";
            string link = EditUrl("pid", p.ProcessServerID.ToString(), "review");
            string body = string.Format("<p>Please review the following application for {0} {1}.</p><p><a href='{2}'>Review Application</a></p>", p.FirstName, p.LastName, link);
            var rCtl = RoleController.Instance;
            var users = rCtl.GetUsersByRole(PortalId, CourtCounselRole);
            
            foreach (var user in users)
            {
                if (user.IsSuperUser == false && user.IsAdmin == false)
                    DotNetNuke.Services.Mail.Mail.SendEmail("process@jud12.flcourts.org", user.Email, subject, body);
            }

        }

        protected void rptProcessServers_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RepeaterItem item = e.Item;
                var sm = ScriptManager.GetCurrent(Page);
                LinkButton cmdRenewal = (LinkButton)item.FindControl("cmdRenewal");
                LinkButton cmdCHCompleted = (LinkButton)item.FindControl("cmdCHCompleted");
                sm.RegisterAsyncPostBackControl(cmdRenewal);
                sm.RegisterAsyncPostBackControl(cmdCHCompleted);
            }
        }

        protected void rptProcessServers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Components.ProcessServer p = (Components.ProcessServer)e.Item.DataItem;
                HyperLink lnkSearch = (HyperLink)e.Item.FindControl("lnkSearch");
                HyperLink lnkEdit = (HyperLink)e.Item.FindControl("lnkEdit");
                lnkSearch.NavigateUrl = EditUrl("pid", p.ProcessServerID.ToString(), "review");
                lnkEdit.NavigateUrl = EditUrl("pid", p.ProcessServerID.ToString(), "edit");
                LinkButton cmdRenewal = (LinkButton)e.Item.FindControl("cmdRenewal");
                LinkButton cmdCHCompleted = (LinkButton)e.Item.FindControl("cmdCHCompleted");
                LinkButton cmdPaid = (LinkButton)e.Item.FindControl("cmdPaid");
                Literal ltStatus = (Literal)e.Item.FindControl("ltStatus");
                Literal ltExpirationDate = (Literal)e.Item.FindControl("ltExpirationDate");
                if (p.ExpiredOrNoValue && p.ExpirationDate.HasValue)
                    ltExpirationDate.Text = string.Format("<span class='text-danger'>{0}</span>", p.ExpirationDate.Value.ToShortDateString());
                else if (p.ExpirationDate.HasValue)
                    ltExpirationDate.Text = string.Format("<span>{0}</span>", p.ExpirationDate.Value.ToShortDateString());
                cmdPaid.Text = p.PaymentReceivedDate.HasValue ? "<em data-label='Active' class='fa fa-check-square'></em>" : "<em data-label='InActive' class='fa fa-square'></em>";
                cmdRenewal.Text = p.IsRenewal ? "<em data-label='Active' class='fa fa-check-square'></em>" : "<em data-label='InActive' class='fa fa-square'></em>";
                cmdCHCompleted.Text = p.CHCompleted ? "<em data-label='Active' class='fa fa-check-square'></em>" : "<em data-label='InActive' class='fa fa-square'></em>";
                ltStatus.Text = string.Format("<abbr data-bs-toggle='tooltip' title='{1}'>{0}</abbr>", p.StatusText, p.StatusTooltip);
                if (p.Status >= applicationStatus.CriminalHistoryComplete)
                {
                    cmdCHCompleted.OnClientClick = @"return confirm('Are you sure?\n\nThis will Reset the Application to New and remove all current approvals?')";
                }
            }
        }
        protected void updateProcessServer_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptTransactions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Components.Transaction t = (Components.Transaction)e.Item.DataItem;
                if (!t.Success)
                {
                    HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("tRow");
                    trRow.Attributes.Add("class", "text-danger");
                }
            }
        }

        protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }
        #endregion


    }
}