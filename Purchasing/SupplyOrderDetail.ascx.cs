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
using DotNetNuke.Services.FileSystem;
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
    public partial class SupplyOrderDetail : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private string _currentProtocol;
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
        public SupplyOrderDetail()
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
                _currentProtocol = Request.IsSecureConnection ? "https://" : "http://";
                if (!IsPostBack)
                {
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    if (CurrentOrderId > 0)
                    {
                        hdOrderId.Value = CurrentOrderId.ToString();
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
                            txtEmail.Text = order.EmailAddress;
                            BindSupplysList(OrderId);
                            ltAttachments.Text= BuildAttachments(order.SupplyOrderAttachments);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("supply-list"), true);
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
            }
            else
            {
                formLine.OrderID = orderId;
                ctl.CreateSupplyOrderItem(formLine);
                supplyId = formLine.SupplyID;
            }
            cmdSave.Visible = true;
            BindSupplysList(orderId);
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
                    order.EmailAddress = txtEmail.Text;
                    order.Location = drpLocation.SelectedValue;
                    ctl.UpdateSupplyOrder(order);
                    SendEmail(order);
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                    return;
                }
                Response.Redirect(EditUrl("supply-list"), true);
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                    "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("Please add Supply Items to Order before submitting") + "', type: 'warning', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
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
        private string BuildAttachments(IEnumerable<SupplyOrderAttachment> attachments)
        {
            string currentProtocol = Request.IsSecureConnection ? "https://" : "http://";

            string attachementList = string.Empty;
            FileManager objFile = new FileManager();
            int attachmentCount = 0;
            foreach (SupplyOrderAttachment f in attachments)
            {
                var file = objFile.GetFile(f.FileID);
                if (file != null)
                    attachementList += string.Format("<li><a href='{0}{1}/portals/{2}/{3}' target='_blank' title='{4}'>attachment #{5}</a></li>", currentProtocol, PortalAlias.HTTPAlias, PortalId, file.RelativePath, file.FileName, ++attachmentCount);
            }
            return attachementList;

        }
        private void SendEmail(Components.SupplyOrder supplyOrder)
        {
            string subject = "Supply Order Form";
            string fromAddress = "purchasing@jud12.flcourts.org";
            string currentProtocol = Request.IsSecureConnection ? "https://" : "http://";
            StringBuilder sb = new StringBuilder();
            sb.Append("<h2>Supply Order Details</h2>");
            sb.Append("<ul style='list-style:none;margin:0;padding:0'><li><strong>Requested By: </strong>");
            sb.Append(supplyOrder.RequestedName);
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
                sb.Append(BuildAttachments(attachments));
                sb.Append("</ul>");
            }
            subject = "Supply Order Updated by Purchasing";
            DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, "purchasing@jud12.flcourts.org", txtEmail.Text, subject, sb.ToString());
        }
        protected void BindSupplysList(int orderId)
        {
            var ctl = new SupplyOrderController();
            rptSupplies.DataSource = ctl.GetSupplyOrderItemsByOrder(orderId);
            rptSupplies.DataBind();
        }

        #endregion
    }
}