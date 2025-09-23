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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing
{
    public partial class SupplyOrder : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private string currentProtocol;
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
        public SupplyOrder()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        #region Event Handlers    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.Title = "Supply Order";
            PortalSettings.ActiveTab.Title = Page.Title;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                attachmentHandler = TemplateSourceDirectory + "/Handlers/AttachmentHandler.ashx";
                currentProtocol = Request.IsSecureConnection ? "https://" : "http://";
                if (!IsPostBack)
                {
                    cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                    lnkCancelLine.NavigateUrl = _navigationManager.NavigateURL();
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                    if (UserId > 0)
                    {
                        txtRequestor.Text = UserInfo.DisplayName;
                        txtEmail.Text = UserInfo.Email;
                        if (UserInfo.IsInRole(AdminRole))
                        {
                            lnkAdmin.NavigateUrl = EditUrl("supply-list");
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
                            hdSupplyId.Value = CurrentItemId.ToString();
                        }
                        var ctl = new SupplyOrderController();
                        var order = ctl.GetSupplyOrder(OrderId);
                        if (order != null)
                        {
                            txtRequestor.Text = order.RequestedName;
                            drpLocation.SelectedValue = order.Location;
                            BindSupplysList(OrderId);
                            SetTargetFolder();
                        }
                        else
                        {
                            BindSupplysList(0);
                        }
                    }
                    else { BindSupplysList(0); }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptSupplies_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Int32.TryParse(e.CommandArgument.ToString(), out int supplyId);
            var ctl = new SupplyOrderController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteSupplyOrderItem(supplyId);
                BindSupplysList(OrderId);
            }
            Response.Redirect(EditUrl("oid", OrderId.ToString(), "detail"), true);
        }

        protected void rptSupplies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (rptSupplies.Items.Count < 1)
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    Literal lblFooter = (Literal)e.Item.FindControl("ltEmptyMessage");
                    lblFooter.Visible = true;
                }
            }
        }

        protected void cmdAddSupply_Click(object sender, EventArgs e)
        {
            int orderId = 0;
            if (!string.IsNullOrEmpty(hdOrderId.Value))
            {
                orderId = int.Parse(hdOrderId.Value);
            }
            var ctl = new SupplyOrderController();
            Components.SupplyOrder order = new Components.SupplyOrder();

            int supplyId = 0;
            if (!string.IsNullOrEmpty(hdSupplyId.Value))
            {
                supplyId = int.Parse(hdSupplyId.Value);
            }

            int quantity = 0;
            if (orderId > 0)
            {
                order = ctl.GetSupplyOrder(orderId);
            }
            order.DateRequested = DateTime.Now;
            order.RequestedName = txtRequestor.Text;
            order.EmailAddress = txtEmail.Text;
            order.Location = drpLocation.SelectedValue;
            if (orderId > 0)
            {
                ctl.UpdateSupplyOrder(order);
            }
            else
            {
                ctl.CreateSupplyOrder(order);
                SendInitialEmail(order);
                orderId = order.OrderID;
                hdOrderId.Value = orderId.ToString();
            }
            SupplyOrderItem formLine = new SupplyOrderItem();
            if (supplyId > 0)
            {
                formLine = ctl.GetSupplyOrderItem(supplyId);
            }
            if (!string.IsNullOrEmpty(txtQuantity.Text))
            {
                quantity = int.Parse(txtQuantity.Text);
            }
            formLine.Comments = txtComments.Text;
            formLine.Link = txtLink.Text;
            formLine.Description = txtDescription.Text;
            formLine.Store = txtStore.Text;
            formLine.CreatedDate = DateTime.Now;
            formLine.ItemNumber = txtSupplyNumber.Text;
            formLine.Quantity = quantity;
            formLine.Recipient = txtRecipient.Text;
            formLine.UnitOfMeasure = txtUnitsOfMeasure.Text;
            if (supplyId > 0)
            {
                ctl.UpdateSupplyOrderItem(formLine);
                AddAttachments(orderId, supplyId);
            }
            else
            {
                formLine.OrderID = orderId;
                ctl.CreateSupplyOrderItem(formLine);
                supplyId = formLine.SupplyID;
                if (supplyId > 0)
                    AddAttachments(orderId, supplyId);
            }
            cmdSave.Enabled = true;
            BindSupplysList(orderId);
            ClearForm();
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (OrderId > 0)
            {
                var ctl = new SupplyOrderController();
                var order = ctl.GetSupplyOrder(OrderId);
                try
                {
                    order.RequestedName = txtRequestor.Text;
                    order.Location = drpLocation.SelectedValue;
                    ctl.UpdateSupplyOrder(order);
                    SendEmail(order);
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                    return;
                }
                Response.Redirect(EditUrl("Supply", "form", "complete"), true);
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Page, "Please add Supply Items to Order before submitting", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }

        protected void valUpload_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            if (hdAttachmentIds.Value != string.Empty)
            {
                args.IsValid = true;
            }
        }

        #endregion
      
        #region Methods
        private void SendEmail(Components.SupplyOrder supplyOrder)
        {
            string subject = "Supply Order Form ";
            string fromAddress = "webhelp@jud12.flcourts.org";
            string currentProtocol = Request.IsSecureConnection ? "https://" : "http://";
            DotNetNuke.Services.FileSystem.FileManager dCtl = (DotNetNuke.Services.FileSystem.FileManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFileManager, DotNetNuke.Services.FileSystem.FileManager>.Instance;
            StringBuilder sb = new StringBuilder();
            sb.Append("<h2>Supply Order Details</h2>");
            sb.Append("<ul style='list-style:none;margin:0;padding:0'><li><strong>Requested By: </strong>");
            if (string.IsNullOrEmpty(supplyOrder.EmailAddress))
                string.Format("<a href='mailto:{0}'>", supplyOrder.EmailAddress);
            sb.Append(supplyOrder.RequestedName);
            if (string.IsNullOrEmpty(supplyOrder.EmailAddress))
                sb.Append("</a>");
            sb.Append("<li><strong>Order Id: </strong>");
            sb.Append(supplyOrder.OrderID.ToString());
            sb.Append("</li><li><strong>Location: </strong> ");
            sb.Append(supplyOrder.Location);
            sb.Append("</li></ul><h3>Order Lines</h3><table cellspacing='0' cellpadding='5' border='1'><thead><tr><th>Item #</th><th>Store<th>Description</th><th>Qty</th><th>Units of Measure</th><th>End User</th><th>Comments</th></tr></thead><tbody>");
            foreach (var item in supplyOrder.SupplyOrderItems)
            {
                sb.Append("<tr><td>");
                sb.Append(item.ItemNumber);
                sb.Append("</td><td>");
                sb.Append(item.Store);
                sb.Append("</td><td>");
                sb.Append(item.LinkedDescription);
                sb.Append("</td><td>");
                sb.Append(item.Quantity.ToString());
                sb.Append("</td><td>");
                sb.Append(item.UnitOfMeasure);
                sb.Append("</td><td>");
                sb.Append(item.Recipient);
                sb.Append("</td><td>");
                sb.Append(item.Comments);
                sb.Append("</td></tr>");
            }
            sb.Append("</tbody></table>");
            IEnumerable<SupplyOrderAttachment> attachments = supplyOrder.SupplyOrderAttachments;
            if (attachments.Count() > 0)
            {
                sb.Append("<h3>Attachments</h3><ul>");
                foreach (SupplyOrderAttachment attach in attachments)
                    if (attach.FileID > 0)
                    {
                        var fileInfo = dCtl.GetFile(attach.FileID);
                        sb.Append(string.Format("<li><a target='_blank' title='Opens in new tab' href='{0}{1}/portals/{2}/{3}'>{4}</a></li>", currentProtocol, PortalAlias.HTTPAlias, PortalId, fileInfo.RelativePath, fileInfo.FileName));
                    }
                sb.Append("</ul>");
            }
            subject = string.Format("Supply Order Form for {0}", supplyOrder.RequestedName);
            DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, "webhelp@jud12.flcourts.org", EmailList, subject, sb.ToString());
            subject = "Supply Order Confirmation";
            SendConfirmationEmail(fromAddress, supplyOrder.EmailAddress, subject, sb.ToString());
        }
        private void SendConfirmationEmail(string fromAddress,string toAddress,string subject,string body)
        {

            DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, toAddress, txtEmail.Text, subject, body);
        }
        private void SendInitialEmail(Components.SupplyOrder supplyOrder)
        {
            string subject = "Supply Order Notification";
            string fromAddress = "webhelp@jud12.flcourts.org";
            string currentProtocol = Request.IsSecureConnection ? "https://" : "http://";
            DotNetNuke.Services.FileSystem.FileManager dCtl = (DotNetNuke.Services.FileSystem.FileManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFileManager, DotNetNuke.Services.FileSystem.FileManager>.Instance;
            StringBuilder sb = new StringBuilder();
            sb.Append("<h2>Supply Order Notification</h2>");
            sb.Append("<ul style='list-style:none;margin:0;padding:0'><li><strong>Requested By: </strong>");
            if (string.IsNullOrEmpty(supplyOrder.EmailAddress))
                string.Format("<a href='mailto:{0}'>", supplyOrder.EmailAddress);
            sb.Append(supplyOrder.RequestedName);
            if (string.IsNullOrEmpty(supplyOrder.EmailAddress))
                sb.Append("</a>");
            sb.Append("<li><strong>Order Id: </strong>");
            sb.Append(supplyOrder.OrderID.ToString());
            sb.Append("</li><li><strong>Location: </strong> ");
            sb.Append(supplyOrder.Location);
            sb.Append("</li></ul>");
            DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, "webhelp@jud12.flcourts.org", EmailList, subject, sb.ToString());
        }

        protected void BindSupplysList(int orderId)
        {
            var ctl = new SupplyOrderController();
            rptSupplies.DataSource = ctl.GetSupplyOrderItemsByOrder(orderId);
            rptSupplies.DataBind();
        }
        private void ClearForm()
        {
            hdSupplyId.Value = string.Empty;
            txtRecipient.Text = string.Empty;
            txtStore.Text = string.Empty;
            txtSupplyNumber.Text = string.Empty;
            txtLink.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtUnitsOfMeasure.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtComments.Text = string.Empty;
        }
        protected void AddAttachments(int orderId, int supplyId)
        {
            var ctl = new AttachmentController();
            if (!string.IsNullOrEmpty(hdAttachmentIds.Value))
            {
                var fileIds = hdAttachmentIds.Value.Split('|');
                foreach (string fileId in fileIds)
                {
                    Int32.TryParse(fileId, out int FileID);
                    SupplyOrderAttachment attachment = new SupplyOrderAttachment { FileID = FileID, OrderID = orderId };
                    ctl.CreateSupplyAttachment(attachment);
                }
                hdAttachmentIds.Value = string.Empty;
            }
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