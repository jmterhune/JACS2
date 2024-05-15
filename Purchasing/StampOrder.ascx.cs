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

namespace tjc.Modules.Purchasing
{
    public partial class StampOrder : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public StampOrder()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    cmdCancel.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL();
                    SetTargetFolder();
                    if (UserInfo != null && UserInfo.IsInRole(AdminRole))
                    {
                        lnkAdmin.NavigateUrl = EditUrl("list");
                        lnkAdmin.Visible = true;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }
        protected void SetTargetFolder()
        {
            DotNetNuke.Services.FileSystem.FolderManager dCtl = (DotNetNuke.Services.FileSystem.FolderManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFolderManager, DotNetNuke.Services.FileSystem.FolderManager>.Instance;
            if (!dCtl.FolderExists(PortalId, CoTargetFolder))
            {
                var folder = dCtl.AddFolder(PortalId, CoTargetFolder);
            }
            rdUpload.TargetFolder = "/portals/" + PortalId.ToString() + "/" + CoTargetFolder;
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            string subject = "Custom Stamp Order Form";
            string fromAddress = "noreply.intranet@jud12.flcourts.org";
            var order = new OfficeForms.Components.StampOrder() { DateCreated = DateAndTime.Now, RequestedName = txtRequestor.Text, ConsumerName = txtConsumerName.Text, Phone = txtPhone.Text, StampType = drpStampType.SelectedValue, Sample = txtSample.Text, FontStyle = drpFontStyle.SelectedValue, FontSize = txtFontSize.Text, InkColor = drpInkColor.SelectedValue, Instructions = txtInstructions.Text, Quantity = int.Parse(txtQuantity.Text), Location = drpLocation.SelectedValue, EmailAddress = txtEmailAddress.Text };
            var ctl = new OfficeForms.Components.Controller();
            var sb = new StringBuilder();
            try
            {
                int orderId = ctl.AddOrder(order);
                UploadFiles(orderId);
                if (orderId > 0)
                {
                    sb.Append("<h2>Stamp Order Details</h2>");
                    sb.Append("<ul style='list-style:none;margin:0;padding:0'><li><strong>Requested By: </strong>");
                    sb.Append(order.RequestedName);
                    sb.Append("</li><li><strong>Order Id: </strong>");
                    sb.Append(orderId);
                    sb.Append("</li><li><strong>Phone: </strong>");
                    sb.Append(order.Phone);
                    sb.Append("</li><li><strong>Email: </strong>");
                    sb.Append(order.EmailAddress);
                    sb.Append("</li><li><strong>Delivery Location: </strong>");
                    sb.Append(order.Location);
                    sb.Append("</li><li><strong>Stamp is For: </strong>");
                    sb.Append(order.ConsumerName);
                    sb.Append("</li><li><strong>Type of Stamp: </strong>");
                    sb.Append(order.StampType);
                    sb.Append("</li><li><strong>Sample: </strong>");
                    sb.Append(GetSample());
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
                    sb.Append("</li></ul>");
                    sb.Append("<h3>Attachments</h3><ul>");
                    string currentProtocol = Conversions.ToString(Interaction.IIf(Request.IsSecureConnection, "https://", "http://"));
                    foreach (var attach in ctl.GetCoAttachmentsByOrder(orderId))
                        sb.Append(string.Format("<li><a href='{0}{1}/portals/0/{2}'>{3}</a></li>", currentProtocol, PortalSettings.PortalAlias.HTTPAlias, attach.Path, attach.FileName));
                    sb.Append("</ul>");

                    DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, "webhelp@jud12.flcourts.org", EmailList, subject, sb.ToString());
                    Response.Redirect(EditUrl("complete"), true);
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
        protected void UploadFiles(int orderId)
        {
            try
            {
                if (rdUpload.UploadedFiles.Count > 0)
                {
                    DotNetNuke.Services.FileSystem.FileManager fCtl = (DotNetNuke.Services.FileSystem.FileManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFileManager, DotNetNuke.Services.FileSystem.FileManager>.Instance;
                    DotNetNuke.Services.FileSystem.FolderManager dCtl = (DotNetNuke.Services.FileSystem.FolderManager)DotNetNuke.ComponentModel.ComponentBase<DotNetNuke.Services.FileSystem.IFolderManager, DotNetNuke.Services.FileSystem.FolderManager>.Instance;
                    var ctl = new OfficeForms.Components.Controller();
                    var folder = dCtl.GetFolder(PortalId, CoTargetFolder);
                    foreach (Telerik.Web.UI.UploadedFile f in rdUpload.UploadedFiles)
                    {
                        try
                        {
                            string fileName = string.Format("{0}-{1}", orderId, f.FileName);
                            var fileInfo = fCtl.AddFile(folder, fileName, f.InputStream);
                            var objAttachment = new OfficeForms.Components.Attachment() { FileID = fileInfo.FileId, FileName = fileName, OrderID = orderId, Path = fileInfo.RelativePath };
                            ctl.AddCoAttachment(objAttachment);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
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
            string output = string.Format("<div style='color:{0};font-size:{1};font-family:{2}'>", color, fontsize, fontStyle);
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
                valFontSize.Visible = true;
                valFontColor.Visible = true;
                drpInkColor.CssClass = "dnnFormRequired dnnFormInput";
                drpFontStyle.CssClass = "dnnFormRequired dnnFormInput";
                txtFontSize.CssClass = "dnnFormRequired";
            }
            else
            {
                valFontStyle.Visible = false;
                valFontSize.Visible = false;
                valFontColor.Visible = false;
                drpInkColor.CssClass = "dnnFormInput";
                drpFontStyle.CssClass = "dnnFormInput";
                txtFontSize.CssClass = "";
            }
        }

    }
}