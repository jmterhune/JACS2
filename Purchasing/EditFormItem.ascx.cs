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
using System.Text;
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
                    cmdCancel.NavigateUrl = EditUrl("list");
                    if (CurrentOrderId > 0)
                    {
                        lnkCancelLine.NavigateUrl = EditUrl("oid", CurrentOrderId.ToString(), "detail");
                        hdOrderId.Value = CurrentOrderId.ToString();
                        if (CurrentFormId > 0)
                        {
                            hdFormId.Value = CurrentFormId.ToString();
                        }
                        var ctl = new FormOrderController();
                        var aCtl=new AttachmentController();
                        var order = ctl.GetFormOrder(CurrentOrderId);
                        if (order != null)
                        {
                            txtRequestor.Text = order.RequestedName;
                            drpLocation.SelectedValue = order.Location;
                            var formLine = ctl.GetFormOrderItem(CurrentFormId);
                            if (formLine != null)
                            {
                                cmdAddForm.Text = "Add Additional Form";
                                txtFormNumber.Text = formLine.FormNumber;
                                txtFormName.Text = formLine.FormName;
                                txtDescription.Text = formLine.Description;
                                txtQuantity.Text = formLine.Quantity.ToString();
                                txtRecipient.Text = formLine.Recipient;
                                txtComments.Text = formLine.Comments;
                                cmdAddForm.Text = "Update Form";
                            }
                            rptForms.DataSource = ctl.GetFormOrderItemsByOrder(CurrentOrderId);
                            rptForms.DataBind();
                            rptFiles.DataSource = aCtl.GetAttachmentsByOrder(CurrentOrderId);
                            rptFiles.DataBind();
                            if (rptFiles.Items.Count < 1)
                            {
                                rptFiles.Visible = false;
                            }

                        }
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
            string storeLink = "";
            int orderId = 0;
            if (!string.IsNullOrEmpty(hdOrderId.Value))
            {
                orderId = int.Parse(hdOrderId.Value);
            }
            var order = new Components.FormOrder();

            int formid = 0;
            if (!string.IsNullOrEmpty(hdFormId.Value))
            {
                formid = int.Parse(hdFormId.Value);
            }

            int quantity = 0;
            var ctl = new FormOrderController();
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
            var formLine = new FormOrderItem();
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
                formid = formLine.OrderID;
            }
            Response.Redirect(EditUrl("oid", CurrentOrderId.ToString(), "detail"), true);
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            string storeLink = "";
            if (CurrentOrderId > 0)
            {
                var ctl = new FormOrderController();
                var order = ctl.GetFormOrder(CurrentOrderId);
                var sb = new StringBuilder();
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
                FormOrderItem item = (FormOrderItem)e.Item.DataItem;
                HyperLink lnkItemEdit = (HyperLink)e.Item.FindControl("lnkItemEdit");
                lnkItemEdit.NavigateUrl = EditUrl("oid", CurrentOrderId.ToString(), "detail", "id=" + item.FormID);
            }

        }
     #endregion
    }

}