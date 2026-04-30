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
using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Mail;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing
{
    public partial class FormOrder : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private string _currentProtocol;
        public string attachmentHandler = "";

        public int OrderId
        {
            get
            {
                if (string.IsNullOrEmpty(hdOrderId.Value))
                {
                    return 0;
                }
                else
                {
                    return Int32.Parse(hdOrderId.Value);
                }
            }
        }
        public FormOrder()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();

        }
        private void ClearForm()
        {
            txtFormNumber.Text = string.Empty;
            hdFormId.Value = string.Empty;
            txtComments.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtFormName.Text = string.Empty;
            drpNumberParts.SelectedIndex = 0;
            txtNumberSets.Text = string.Empty;
            drpPageType.SelectedIndex = 0;
            txtRecipient.Text = string.Empty;
        }
        #region Event Handlers    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.Title = "Form Order";
            PortalSettings.ActiveTab.Title = Page.Title;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                attachmentHandler = TemplateSourceDirectory + "/Handlers/AttachmentHandler.ashx";
                _currentProtocol = Request.IsSecureConnection ? "https://" : "http://";
                if (!IsPostBack)
                {
                    cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                    lnkCancelLine.NavigateUrl = _navigationManager.NavigateURL();
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                    if (UserInfo != null)
                    {
                        txtRequestor.Text = UserInfo.DisplayName;
                        txtEmailAddress.Text = UserInfo.Email;
                        if (UserInfo.IsInRole(AdminRole))
                        {
                            lnkAdmin.NavigateUrl = EditUrl("form-list");
                            lnkAdmin.Visible = true;
                            divComments.Visible = true;
                        }
                    }
                    if (CurrentOrderId > 0)
                    {
                        hdOrderId.Value = CurrentOrderId.ToString();
                        lnkCancelLine.NavigateUrl = string.Format("{0}?oid={1}", _navigationManager.NavigateURL(), OrderId);
                        if (CurrentItemId > 0)
                        {
                            hdFormId.Value = CurrentItemId.ToString();
                        }
                        var ctl = new FormOrderController();
                        var order = ctl.GetFormOrder(OrderId);
                        if (order != null)
                        {
                            txtRequestor.Text = order.RequestedName;
                            txtEmailAddress.Text = order.EmailAddress;
                            drpLocation.SelectedValue = order.Location;
                            BindFormsList(OrderId);
                            SetTargetFolder();
                        }
                        else
                        {
                            BindFormsList(0);
                        }
                    }
                    else { BindFormsList(0); }
                    if (UserId > 0)
                    {
                        txtRequestor.Text = UserInfo.DisplayName;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (OrderId > 0)
            {
                var ctl = new FormOrderController();
                var aCtl = new AttachmentController();
                DotNetNuke.Services.FileSystem.FileManager dCtl = (DotNetNuke.Services.FileSystem.FileManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFileManager, DotNetNuke.Services.FileSystem.FileManager>.Instance;
                var order = ctl.GetFormOrder(OrderId);
                string fromAddress = "purchasing@jud12.flcourts.org";
                StringBuilder sb = new StringBuilder();
                string subject = "";
                order.DateRequested = DateTime.Now;
                try
                {
                    order.RequestedName = txtRequestor.Text;
                    order.EmailAddress = txtEmailAddress.Text;
                    order.Location = drpLocation.SelectedValue;
                    ctl.UpdateFormOrder(order);
                    sb.Append("<h2>Court Form Order  Details</h2>");
                    sb.Append("<ul style='list-style:none;margin:0;padding:0'><li><strong>Requested By: </strong>");
                    if (!string.IsNullOrEmpty(order.EmailAddress))
                        sb.Append(string.Format("<a href='mailto:{0}'>", order.EmailAddress));
                    sb.Append(order.RequestedName);
                    if (!string.IsNullOrEmpty(order.EmailAddress))
                        sb.Append("</a>");
                    sb.Append("<li><strong>Order Id: </strong>");
                    sb.Append(order.OrderID);
                    sb.Append("</li><li><strong>Location: </strong> ");
                    sb.Append(order.Location);
                    sb.Append("</li></ul><h3>Form Order Lines</h3><table cellspacing='0' cellpadding='5' border='1'><thead><tr><th>Form #</th><th>Form Name</th><th># Sets</th><th># Parts</th><th>Page Size</th><th>Description</th><th>End User</th><th>Comments</th></tr></thead><tbody>");
                    foreach (var formItem in ctl.GetFormOrderItemsByOrder(order.OrderID))
                    {
                        sb.Append("<tr><td>");
                        sb.Append(formItem.FormNumber);
                        sb.Append("</td><td>");
                        sb.Append(formItem.FormName);
                        sb.Append("</td><td>");
                        sb.Append(formItem.Quantity);
                        sb.Append("</td><td>");
                        sb.Append(formItem.NumberParts);
                        sb.Append("</td><td>");
                        sb.Append(formItem.PageType);
                        sb.Append("</td><td>");
                        sb.Append(formItem.Description);
                        sb.Append("</td><td>");
                        sb.Append(formItem.Recipient);
                        sb.Append("</td><td>");
                        sb.Append(formItem.Comments);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td colspan='9'><h3>Attachments</h3><ul>");
                        foreach (var attach in aCtl.GetFormAttachmentsByFormId(formItem.FormID))
                            if (attach.FileID > 0)
                            {
                                var fileInfo = dCtl.GetFile(attach.FileID);
                                sb.Append(string.Format("<li><a target='_blank' title='Opens in new tab' href='{0}{1}/portals/{2}/{3}'>{4}</a></li>", _currentProtocol, PortalAlias.HTTPAlias, PortalId, fileInfo.RelativePath, fileInfo.FileName));
                            }
                        sb.Append("</ul></td></tr>");
                    }
                    sb.Append("</tbody></table>");
                    sb.Append("</ul>");
                    subject = string.Format("Court Form Order for {0}", order.RequestedName);
                    DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, "purchasing@jud12.flcourts.org", EmailList, subject, sb.ToString());
                    subject = "Form Order Confirmation";
                    DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, "purchasing@jud12.flcourts.org", order.EmailAddress, subject, sb.ToString());
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
                Response.Redirect(EditUrl("form", "form", "complete"), true);
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Page, "Please add Line Forms to Order before submitting", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }
        protected void cmdAddForm_Click(object sender, EventArgs e)
        {
            int orderId = 0;
            if (!string.IsNullOrEmpty(hdOrderId.Value))
            {
                orderId = int.Parse(hdOrderId.Value);
            }
            var ctl = new FormOrderController();
            Components.FormOrder order = new Components.FormOrder();

            int formid = 0;
            if (!string.IsNullOrEmpty(hdFormId.Value))
            {
                formid = int.Parse(hdFormId.Value);
            }

            int quantity = 0;
            if (orderId > 0)
            {
                order = ctl.GetFormOrder(orderId);
            }
            order.DateRequested = DateTime.Now;
            order.RequestedName = txtRequestor.Text;
            order.EmailAddress = txtEmailAddress.Text;
            order.Location = drpLocation.SelectedValue;
            if (orderId > 0)
            {
                ctl.UpdateFormOrder(order);
            }
            else
            {
                ctl.CreateFormOrder(order);
                orderId = order.OrderID;
                hdOrderId.Value = orderId.ToString();
            }
            FormOrderItem formLine = new FormOrderItem();
            if (formid > 0)
            {
                formLine = ctl.GetFormOrderItem(formid);
            }
            if (!string.IsNullOrEmpty(txtNumberSets.Text))
            {
                quantity = int.Parse(txtNumberSets.Text);
            }
            formLine.Comments = txtComments.Text;
            formLine.Description = txtDescription.Text;
            formLine.FormNumber = txtFormNumber.Text;
            formLine.CreatedDate = DateTime.Now;
            formLine.FormName = txtFormName.Text;
            if (drpNumberParts.SelectedValue != "0")
                formLine.NumberParts = Int32.Parse(drpNumberParts.SelectedValue);
            if (drpPageType.SelectedValue != "")
                formLine.PageType = drpPageType.SelectedValue;
            formLine.Quantity = quantity;
            formLine.Recipient = txtRecipient.Text;
            if (formid > 0)
            {
                ctl.UpdateFormOrderItem(formLine);
            }
            else
            {
                formLine.OrderID = orderId;
                ctl.CreateFormOrderItem(formLine);
                formid = formLine.FormID;
            }
            AddAttachments(orderId, formid);
            cmdSave.Enabled = true;
            BindFormsList(orderId);
            ClearForm();
        }
        protected void rptForms_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            Int32.TryParse(e.CommandArgument.ToString(), out int formId);
            var ctl = new FormOrderController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteFormOrderItem(formId);
            }
            Response.Redirect(EditUrl("oid", OrderId.ToString(), "detail"), true);
        }
        protected void valUpload_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            if (hdAttachmentIds.Value != string.Empty)
            {
                args.IsValid = true;
            }
        }

        protected void rptForms_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (rptForms.Items.Count < 1)
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    Literal lblFooter = (Literal)e.Item.FindControl("ltEmptyMessage");
                    lblFooter.Visible = true;
                }
            }
        }
        #endregion
        #region Methods
        protected void BindFormsList(int orderId)
        {
            var ctl = new FormOrderController();
            rptForms.DataSource = ctl.GetFormOrderItemsByOrder(orderId);
            rptForms.DataBind();

        }

        protected void AddAttachments(int orderId, int formId)
        {
            var ctl = new AttachmentController();
            var fileIds = hdAttachmentIds.Value.Split(',');
            foreach (string fileId in fileIds)
            {
                Int32.TryParse(fileId, out int FileID);
                FormOrderAttachment attachment = new FormOrderAttachment { FileID = FileID, FormID = formId, OrderID = orderId };
                ctl.CreateFormAttachment(attachment);
            }
            hdAttachmentIds.Value = string.Empty;
        }
        protected void SetTargetFolder()
        {
            DotNetNuke.Services.FileSystem.FolderManager dCtl = (DotNetNuke.Services.FileSystem.FolderManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFolderManager, DotNetNuke.Services.FileSystem.FolderManager>.Instance;
            if (!dCtl.FolderExists(this.PortalId, this.FoTargetFolder))
            {
                var folder = dCtl.AddFolder(this.PortalId, this.FoTargetFolder);
            }
        }


        #endregion

    }
}