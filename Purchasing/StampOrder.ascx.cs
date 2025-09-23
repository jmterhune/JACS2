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
using DotNetNuke.Entities.Host;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing
{
    public partial class StampOrder : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private string currentProtocol;
        public string attachmentHandler = "";

        public StampOrder()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                attachmentHandler = TemplateSourceDirectory + "/Handlers/AttachmentHandler.ashx";
                currentProtocol = Request.IsSecureConnection ? "https://" : "http://";
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                if (!IsPostBack)
                {
                    cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                    SetTargetFolder();
                    if (UserInfo != null && UserInfo.IsInRole(AdminRole))
                    {
                        lnkAdmin.NavigateUrl = EditUrl("list");
                        lnkAdmin.Visible = true;

                    }
                    if (UserId > 0)
                    {
                        txtRequestor.Text=UserInfo.DisplayName;
                        txtEmailAddress.Text=UserInfo.Email;
                        
                    }
                    if (CurrentOrderId > 0)
                    {
                        PopulateForm(CurrentOrderId);
                        cmdCancel.NavigateUrl = EditUrl("list");
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }
        protected void AddAttachments(int orderId)
        {
            var ctl = new AttachmentController();
            if (!string.IsNullOrEmpty(hdAttachmentIds.Value))
            {
                var fileIds = hdAttachmentIds.Value.Split(',');
                foreach (string fileId in fileIds)
                {
                    Int32.TryParse(fileId, out int FileID);
                    StampOrderAttachment attachment = new StampOrderAttachment {  FileID = FileID, OrderID = orderId };
                    ctl.CreateStampAttachment(attachment);
                }
            }
            hdAttachmentIds.Value = string.Empty;
        }
        protected void PopulateForm(int orderId)
        {
            var ctl = new StampOrderController();
            var order = ctl.GetStampOrder(orderId);
            if (order != null)
            {
                lblupload.Visible = false;
                ltTopMessages.Visible = false;
                ltUploadMessage.Visible = false;
                ltBottomMessage.Visible = false;
                txtRequestor.Text = order.RequestedName;
                txtRequestor.ReadOnly = true;
                txtPhone.Text = order.Phone;
                txtPhone.ReadOnly = true;
                txtEmailAddress.Text = order.EmailAddress;
                txtEmailAddress.ReadOnly = true;
                drpLocation.SelectedValue = order.Location;
                drpLocation.Enabled = false;
                txtConsumerName.Text = order.ConsumerName;
                txtConsumerName.ReadOnly = true;
                drpStampType.SelectedValue = order.StampType;
                drpStampType.Enabled = false;
                drpFontStyle.SelectedValue = order.FontStyle;
                drpFontStyle.Enabled = false;
                txtFontSize.Text = order.FontSize;
                txtFontSize.ReadOnly = true;
                drpInkColor.SelectedValue = order.InkColor;
                drpInkColor.Enabled = false;
                txtQuantity.Text = order.Quantity.ToString();
                txtQuantity.ReadOnly = true;
                txtSample.Text = order.Sample.ToString();
                txtSample.ReadOnly = true;
                txtInstructions.Text = order.Instructions.ToString();
                txtInstructions.ReadOnly = true;
                cmdSave.Visible = false;
                uplAttachments.Visible = false;
                lnkAdmin.Visible = false;
                cmdCancel.Text = "Close";
                string attachments = BuildAttachments(orderId);
                ltAttachments.Text = string.Format(" <ul id=\"attachmentList\" class=\"attachments\">{0}</ul>", attachments);
            }
        }
        protected string BuildAttachments(int orderId)
        {
            string attachementList = string.Empty;
            var aCtl = new AttachmentController();
            IEnumerable<StampOrderAttachment> attachments = aCtl.GetStampAttachmentsByOrderId( orderId);
            FileManager objFile = new FileManager();
            int attachmentCount = 0;
            foreach (StampOrderAttachment f in attachments)
            {
                var file = objFile.GetFile(f.FileID);
                if (file != null)
                    attachementList += string.Format("<li><a href='{0}{1}/portals/{2}/{3}' title='{4}'>attachment #{5}</a></li>", currentProtocol, PortalAlias.HTTPAlias, PortalId, file.RelativePath, file.FileName, ++attachmentCount);
            }
            return attachementList;

        }
        protected void SetTargetFolder()
        {
            DotNetNuke.Services.FileSystem.FolderManager dCtl = (DotNetNuke.Services.FileSystem.FolderManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFolderManager, DotNetNuke.Services.FileSystem.FolderManager>.Instance;
            if (!dCtl.FolderExists(this.PortalId, this.FoTargetFolder))
            {
                var folder = dCtl.AddFolder(this.PortalId, this.FoTargetFolder);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            string fromAddress = "webhelp@jud12.flcourts.org";
            var order = new Components.StampOrder { DateCreated = DateTime.Now, RequestedName = txtRequestor.Text, ConsumerName = txtConsumerName.Text, Phone = txtPhone.Text, StampType = drpStampType.SelectedValue, Sample = txtSample.Text, FontStyle = drpFontStyle.SelectedValue, FontSize = txtFontSize.Text, InkColor = drpInkColor.SelectedValue, Instructions = txtInstructions.Text, Quantity = int.Parse(txtQuantity.Text), Location = drpLocation.SelectedValue, EmailAddress = txtEmailAddress.Text };
            var ctl = new StampOrderController();
            var aCtl = new AttachmentController();
            var sb = new StringBuilder();
            try
            {
                ctl.CreateStampOrder(order);
                int orderId = order.OrderID;
                AddAttachments(orderId);
                if (orderId > 0)
                {
                    string subject = string.Format("Custom Stamp Order Form for {0}", order.RequestedName); 
                    sb.Append("<h2>Stamp Order Details</h2>");
                    sb.Append("<ul style='list-style:none;margin:0;padding:0'><li><strong>Requested By: </strong>");
                    sb.Append(order.RequestedName);
                    sb.Append("</li><li><strong>Order Id: </strong>");
                    sb.Append(orderId);
                    sb.Append("</li><li><strong>Phone: </strong>");
                    sb.Append(order.Phone);
                    sb.Append(string.Format("</li><li><strong>Email: </strong><a href='mailto:{0}'>",order.EmailAddress));
                    sb.Append(order.EmailAddress);
                    sb.Append("</a></li><li><strong>Delivery Location: </strong>");
                    sb.Append(order.Location);
                    sb.Append("</li><li><strong>Stamp is For: </strong>");
                    sb.Append(order.ConsumerName);
                    sb.Append("</li><li><strong>Type of Stamp: </strong>");
                    sb.Append(order.StampType);
                    sb.Append("</li><li><strong>Font Style: </strong>");
                    sb.Append(order.FontStyle);
                    sb.Append("</li><li><strong>Font Size: </strong>");
                    sb.Append(order.FontSize);
                    sb.Append("</li><li><strong>Ink Color: </strong>");
                    sb.Append(order.InkColor);
                    sb.Append("</li><li><strong>Quantity: </strong>");
                    sb.Append(order.Quantity);
                    sb.Append("</li><li><strong>Additional Information:</strong> ");
                    sb.Append(order.Instructions);
                    sb.Append("</li></ul><h3>Stamp Sample</h3>");
                    sb.Append(GetSample());
                    sb.Append("<h3>Attachments</h3><ul>");
                    DotNetNuke.Services.FileSystem.FileManager dCtl = (DotNetNuke.Services.FileSystem.FileManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFileManager, DotNetNuke.Services.FileSystem.FileManager>.Instance;
                    foreach (var attach in aCtl.GetStampAttachmentsByOrderId(orderId))
                    {
                        if (attach.FileID > 0)
                        {
                            var fileInfo = dCtl.GetFile(attach.FileID);
                            sb.Append(string.Format("<li><a href='{0}{1}/portals/{2}/{3}'>{4}</a></li>", currentProtocol, PortalAlias.HTTPAlias,PortalId, fileInfo.RelativePath, fileInfo.FileName));
                        }
                    }
                    sb.Append("</ul>");
                    DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, "webhelp@jud12.flcourts.org", EmailList, subject, sb.ToString());
                    subject = "Stamp Order Confirmation";
                    DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, "webhelp@jud12.flcourts.org", txtEmailAddress.Text, subject, sb.ToString());

                    Response.Redirect(EditUrl("form", "stamp", "complete"), true);
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Unable to Add Record. Please contact the <a href='mailto:helpdesk@jud12.flcourts.org'>help desk</a>.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private object GetSample()
        {
            string color = drpInkColor.SelectedValue;
            string fontsize = txtFontSize.Text + "pt";
            string fontStyle = drpFontStyle.SelectedValue;
            string output = string.Format("<div style='text-align:center;color:{0};font-size:{1};font-family:{2}'>", color, fontsize, fontStyle);
            string[] lines = txtSample.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
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

        protected void drpStampType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpStampType.SelectedValue != "signature")
            {
                valFontStyle.Visible = true;
                lblFontStyle.Text = "Font Style<em>*</em>";
                valFontSize.Visible = true;
                lblFontSize.Text = "Font Size<em>*</em>";
                valFontColor.Visible = true;
                lblInkColor.Text = "Ink Color<em>*</em>";

            }
            else
            {
                valFontStyle.Visible = false;
                valFontSize.Visible = false;
                valFontColor.Visible = false;
                lblFontStyle.Text = "Font Style";
                lblFontSize.Text = "Font Size";
                lblInkColor.Text = "Ink Color";
            }
        }
    }
}