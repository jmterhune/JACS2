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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Web.UI.WebControls;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing
{
    public partial class FormOrder : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private string currentProtocol;
        public string attachmentHandler = "";

        public FormOrder()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        #region Event Handlers      
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
                    if (UserInfo != null && UserInfo.IsInRole(AdminRole))
                    {
                        lnkAdmin.NavigateUrl = EditUrl("list");
                        lnkAdmin.Visible = true;
                        divComments.Visible = true;
                    }
                    if (CurrentOrderId > 0)
                    {
                        lnkCancelLine.NavigateUrl = string.Format("{0}?oid={1}", _navigationManager.NavigateURL(), CurrentOrderId);
                        hdOrderId.Value = CurrentOrderId.ToString();
                        if (CurrentItemId > 0)
                        {
                            hdFormId.Value = CurrentItemId.ToString();
                        }
                        var ctl = new FormOrderController();
                        var order = ctl.GetFormOrder(CurrentOrderId);
                        if (order != null)
                        {
                            txtRequestor.Text = order.RequestedName;
                            drpLocation.SelectedValue = order.Location;
                            var formItemLine = ctl.GetFormOrderItem(CurrentItemId);
                            if (formItemLine != null)
                            {
                                cmdAddForm.Text = "Add Additional Form";
                                txtFormNumber.Text = formItemLine.FormNumber;
                                txtDescription.Text = formItemLine.Description;
                                txtQuantity.Text = formItemLine.Quantity.ToString();
                                txtFormName.Text = formItemLine.FormName;
                                txtRecipient.Text = formItemLine.Recipient;
                                txtComments.Text = formItemLine.Comments;
                                cmdAddForm.Text = "Update Item";
                            }
                            rptForms.DataSource = ctl.GetFormOrderItemsByOrder(CurrentOrderId);
                            rptForms.DataBind();
                            SetTargetFolder();
                        }
                    }
                    else if (UserId > 0)
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

            order.Location = drpLocation.SelectedValue;
            if (orderId > 0)
            {
                ctl.UpdateFormOrder(order);
            }
            else
            {
                ctl.CreateFormOrder(order);
                orderId = order.OrderID;
            }
            FormOrderItem formLine = new FormOrderItem();
            if (formid > 0)
            {
                formLine = ctl.GetFormOrderItem(formid);
            }
            if (!string.IsNullOrEmpty(txtQuantity.Text))
            {
                quantity = int.Parse(txtQuantity.Text);
            }
            formLine.Comments = txtComments.Text;
            formLine.Description = txtDescription.Text;
            formLine.FormNumber = txtFormNumber.Text;
            formLine.CreatedDate = DateTime.Now;
            formLine.FormName = txtFormName.Text;
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
            }
            Response.Redirect(EditUrl("oid", CurrentOrderId.ToString(), "detail"), true);
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (CurrentOrderId > 0)
            {
                var ctl = new FormOrderController();
                var order = ctl.GetFormOrder(CurrentOrderId);
                try
                {
                    order.RequestedName = txtRequestor.Text;
                    order.Location = drpLocation.SelectedValue;
                    ctl.UpdateFormOrder(order);
                    Response.Redirect(EditUrl("list"), true);
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Page, "Please add Line Forms to Order before submitting", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }

        protected void rptForms_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            Int32.TryParse(e.CommandArgument.ToString(), out int formId);
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
                FormOrderItem item = (FormOrderItem)e.Item.DataItem;
                HyperLink lnkItemEdit = (HyperLink)e.Item.FindControl("lnkItemEdit");
                lnkItemEdit.NavigateUrl = EditUrl("oid", CurrentOrderId.ToString(), "detail", "id=" + item.FormID);
            }
        }
        #endregion
        #region Methods
        protected void AddAttachments(int orderId, int formId)
        {
            var ctl = new AttachmentController();
            var fileIds = hdAttachmentIds.Value.Split(',');
            foreach (string fileId in fileIds)
            {
                Int32.TryParse(fileId, out int FileID);
                Attachment attachment = new Attachment { ModuleID = ModuleId, FileID = FileID, FormID = formId, OrderID = orderId };
                ctl.CreateAttachment(attachment);
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