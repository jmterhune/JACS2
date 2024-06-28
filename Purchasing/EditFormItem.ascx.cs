/*
' Copyright (c) 2024  Joe Terhune
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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing
{
    public partial class EditFormItem : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public EditFormItem()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        #region Event Handlers   
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.Title =string.Format( "Form Order #{0}",CurrentOrderId);
            PortalSettings.ActiveTab.Title = Page.Title;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (UserInfo != null && UserInfo.IsInRole(AdminRole) == false)
                    {
                        Response.Redirect(_navigationManager.NavigateURL(), true);
                        return;
                    }
                    cmdCancel.NavigateUrl = EditUrl("form-list");
                    if (CurrentOrderId > 0)
                    {
                        hdOrderId.Value = CurrentOrderId.ToString();
                        var ctl = new FormOrderController();
                        var aCtl = new AttachmentController();
                        var order = ctl.GetFormOrder(CurrentOrderId);
                        if (order != null)
                        {
                            txtRequestor.Text = order.RequestedName;
                            drpLocation.SelectedValue = order.Location;
                            rptForms.DataSource = ctl.GetFormOrderItemsByOrder(CurrentOrderId);
                            rptForms.DataBind();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptForms_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int formId = int.Parse(e.CommandArgument.ToString());
            var ctl = new FormOrderController();

            if (e.CommandName == "delete")
            {
                ctl.DeleteFormOrderItem(formId);
            }
            Response.Redirect(EditUrl("oid", CurrentOrderId.ToString(), "detail"), true);

        }

        protected void rptForms_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FormOrderItem formOrderItem = (FormOrderItem)e.Item.DataItem;
                Literal ltAttachments = (Literal)e.Item.FindControl("ltAttachments");
                if (formOrderItem != null)
                {
                    string attachments = BuildAttachments(formOrderItem.FormID);
                    if (!string.IsNullOrEmpty(attachments))
                        ltAttachments.Text = attachments;
                }
                else { ltAttachments.Text = string.Empty; }
            }
        }
        #endregion
        protected string BuildAttachments(int formId)
        {
            string attachementList = string.Empty;
            var aCtl = new AttachmentController();
            IEnumerable<Attachment> attachments = aCtl.GetAttachmentsByFormId(ModuleId, formId);
            FileManager objFile = new FileManager();
            int attachmentCount = 0;
            foreach (Attachment f in attachments)
            {
                var file = objFile.GetFile(f.FileID);
                if (file != null)
                    attachementList += string.Format("<li><a href='/portals/0/{0}' title='{1}'>attachment #{2}</a></li>", file.RelativePath, file.FileName, ++attachmentCount);
            }
            return attachementList;

        }

    }
}