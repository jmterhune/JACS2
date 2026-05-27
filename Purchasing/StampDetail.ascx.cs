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
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from PurchasingModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class StampDetail : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public StampDetail()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();

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
                    cmdCancel.NavigateUrl = EditUrl("list");
                    var ctl = new StampOrderController();
                    var aCtl = new AttachmentController();
                    var objOrder = ctl.GetStampOrder(CurrentOrderId);
                    if (objOrder != null)
                    {
                        txtConsumerName.Text = objOrder.ConsumerName;
                        txtFontSize.Text = objOrder.FontSize;
                        txtInstructions.Text = objOrder.Instructions;
                        txtPhone.Text = objOrder.Phone;
                        txtQuantity.Text = objOrder.Quantity.ToString();
                        txtRequestor.Text = objOrder.RequestedName;
                        txtSample.Text = objOrder.Sample;
                        drpFontStyle.SelectedValue = objOrder.FontStyle;
                        drpInkColor.SelectedValue = objOrder.InkColor;
                        drpLocation.SelectedValue = objOrder.Location;
                        drpStampType.SelectedValue = objOrder.StampType;
                        txtEmailAddress.Text = objOrder.EmailAddress;
                        IEnumerable<StampOrderAttachment> attachments = objOrder.StampOrderAttachments;
                        if (attachments.Count() > 0)
                        {
                            ltAttachments.Text=BuildAttachments(attachments);
                        }
                        ltSample.Text = GetSample();
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                            "Swal.fire({ title: 'Error', html: '" + System.Web.HttpUtility.JavaScriptStringEncode("Unable to Retrieve Record. Please contact the <a href='mailto:helpdesk@jud12.flcourts.org'>help desk</a>.") + "', icon: 'error', confirmButtonText: 'OK' });", true);
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
            var ctl = new StampOrderController();
            var order = ctl.GetStampOrder(CurrentOrderId);
            order.RequestedName = txtRequestor.Text;
            order.ConsumerName = txtConsumerName.Text;
            order.Phone = txtPhone.Text;
            order.StampType = drpStampType.SelectedValue;
            order.Sample = txtSample.Text;
            order.FontStyle = drpFontStyle.SelectedValue;
            order.FontSize = txtFontSize.Text;
            order.InkColor = drpInkColor.SelectedValue;
            order.Instructions = txtInstructions.Text;
            order.Quantity = Int32.Parse(txtQuantity.Text);
            order.Location = drpLocation.SelectedValue;
            order.EmailAddress = txtEmailAddress.Text;
            try
            {
                ctl.UpdateStampOrder(order);
                Response.Redirect(EditUrl("list"), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdReject_Click(object sender, EventArgs e)
        {
            var ctl = new StampOrderController();
            var order = ctl.GetStampOrder(CurrentOrderId);
            order.Status = OrderStatus.rejected;
            try
            {
                SendRejectionEmail(order);
                ctl.UpdateStampOrder(order);
                Response.Redirect(EditUrl("list"), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void SendRejectionEmail(Components.StampOrder order)
        {
            var sb = new StringBuilder();
            string subject = "Stamp Order Rejected";
            if (order.OrderID > 0)
            {
                sb.Append("<p>The Following Stamp Order has been Rejected: ");
                sb.Append(txtRejectionNotice.Text);
                sb.Append("<br /><br /><strong>Please submit a new corrected order</strong></p>");
                sb.Append("<ul style='list-style:none;margin:0;padding:0'><li><strong>Order Id:</strong> ");
                sb.Append(order.OrderID);
                sb.Append("</li><li><strong>Phone:</strong> ");
                sb.Append(order.Phone);
                sb.Append("</li><li><strong>Email:</strong> ");
                sb.Append(order.EmailAddress);
                sb.Append("</li><li><strong>Delivery Location:</strong> ");
                sb.Append(order.Location);
                sb.Append("</li><li><strong>Stamp is For:</strong> ");
                sb.Append(order.ConsumerName);
                sb.Append("</li><li><strong>Type of Stamp:</strong> ");
                sb.Append(order.StampType);
                sb.Append("</li><li><strong>Sample:</strong> ");
                sb.Append(GetSample());
                sb.Append("</li><li><strong>Font Style:</strong> ");
                sb.Append(order.FontStyle);
                sb.Append("</li><li><strong>Font Size:</strong> ");
                sb.Append(order.FontSize);
                sb.Append("</li><li><strong>Ink Color:</strong> ");
                sb.Append(order.InkColor);
                sb.Append("</li><li><strong>Quantity:</strong> ");
                sb.Append(order.Quantity);
                sb.Append("</li><li><strong>Additional Information or Purchasing:</strong> ");
                sb.Append(order.Instructions);
                sb.Append("</li></ul>");
                DotNetNuke.Services.Mail.Mail.SendEmail(EmailList, order.EmailAddress, subject, sb.ToString());
            }
        }
        private string GetSample()
        {
            string color = drpInkColor.SelectedValue;
            string fontsize = txtFontSize.Text + "pt";
            string fontStyle = drpFontStyle.SelectedValue;
            string output = string.Format("<div style='text-align:center;color:{0};line-height:normal;font-size:{1};font-family:{2}'>", color, fontsize, fontStyle);
            string[] lines = txtSample.Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            int samplelength = lines.Length;
            foreach (string line in lines)
            {
                samplelength -= 1;
                if (samplelength == 0)
                {
                    output += line;
                }
                else
                {
                    output += line + "<br>";
                }
            }
            return output + "</div>";
        }
        private string BuildAttachments(IEnumerable<StampOrderAttachment> attachments)
        {
            string currentProtocol = Request.IsSecureConnection ? "https://" : "http://";
            string attachementList = string.Empty;
            FileManager objFile = new FileManager();
            int attachmentCount = 0;
            foreach (StampOrderAttachment f in attachments)
            {
                var file = objFile.GetFile(f.FileID);
                if (file != null)
                    attachementList += string.Format("<li><a href='{0}{1}/portals/{2}/{3}' target='_blank' title='{4}'>attachment #{5}</a></li>", currentProtocol, PortalAlias.HTTPAlias, PortalId, file.RelativePath, file.FileName, ++attachmentCount);
            }
            return string.Format("<h3 class='mb-1'>Attachments</h3><ul class='list'>{0}</ul>", attachementList);
        }
    }
}