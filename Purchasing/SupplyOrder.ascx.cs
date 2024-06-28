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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Web.UI.WebControls;
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

                    if (UserInfo != null)
                    {
                        txtRequestor.Text = UserInfo.DisplayName;
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
        protected void rptSupplies_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Int32.TryParse(e.CommandArgument.ToString(), out int supplyId);
            var ctl = new SupplyOrderController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteSupplyOrderItem(supplyId);
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
            formLine.UnitOfMeasure=txtUnitsOfMeasure.Text;
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
            AddAttachments(orderId, supplyId);
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
                    order.Location = drpLocation.SelectedValue;
                    ctl.UpdateSupplyOrder(order);
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
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
        protected void BindSupplysList(int orderId)
        {
            var ctl = new SupplyOrderController();
            rptSupplies.DataSource = ctl.GetSupplyOrderItemsByOrder(orderId);
            rptSupplies.DataBind();
        }

        protected void AddAttachments(int orderId, int supplyId)
        {
            var ctl = new AttachmentController();
            var fileIds = hdAttachmentIds.Value.Split(',');
            foreach (string fileId in fileIds)
            {
                Int32.TryParse(fileId, out int FileID);
                Attachment attachment = new Attachment { ModuleID = ModuleId, FileID = FileID, FormID = supplyId, OrderID = orderId };
                ctl.CreateAttachment(attachment);
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