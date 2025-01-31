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
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing
{
    public partial class ViewForm : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public ViewForm()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.Title = "Manage Form Orders";
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
                    txtStartDate.Text = DateTime.Now.AddDays(-30).ToShortDateString();
                    txtEndDate.Text = DateTime.Now.ToShortDateString();
                    BindData();
                    lnkReset.NavigateUrl = EditUrl("form-list");
                    lnkForm.NavigateUrl = _navigationManager.NavigateURL();

                }
                chkShowCompleted.InputAttributes.Add("class", "form-check-input");
                chkShowCompleted.LabelAttributes.Add("class", "form-check-label");
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindData()
        {
            DateTime.TryParse(txtStartDate.Text, out DateTime startDate);
            DateTime.TryParse(txtEndDate.Text, out DateTime endDate);
            var ctl = new FormOrderController();
            var orders = ctl.GetFormOrders(startDate, endDate.AddDays(1));
            if (chkShowCompleted.Checked)
            {
                rptOrders.DataSource = orders.OrderByDescending(x=>x.OrderID);
            }
            else
            {
                rptOrders.DataSource = orders.Where(x => x.CompletedDate.HasValue == false).OrderByDescending(x => x.OrderID);
            }
            rptOrders.DataBind();
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int orderId = int.Parse(e.CommandArgument.ToString());
            var ctl = new FormOrderController();

            if (e.CommandName == "toggle")
            {
                var objOrder = ctl.GetFormOrder(orderId);
                if (objOrder != null)
                {
                    if (objOrder.CompletedDate != null)
                    {
                        objOrder.CompletedDate = null;
                    }
                    else
                    {
                        objOrder.CompletedDate = DateTime.Now;
                    }
                    ctl.UpdateFormOrder(objOrder);
                }
            }
            if (e.CommandName == "delete")
            {
                ctl.DeleteFormOrder(ModuleId, orderId);
            }
            BindData();
        }
        public bool IsNullDate(DateTime inDate)
        {
            return inDate == null;
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Components.FormOrder item = (Components.FormOrder)e.Item.DataItem;
                Literal ltFormItems = (Literal)e.Item.FindControl("ltFormItems");
                Components.FormOrder formOrder = (Components.FormOrder)e.Item.DataItem;
                if (formOrder != null)
                {
                    HyperLink lnkDetails = (HyperLink)e.Item.FindControl("lnkDetails");
                    lnkDetails.NavigateUrl = EditUrl("oid", item.OrderID.ToString(), "detail");
                    string formOrderLines = BuildFormOrderLines(formOrder.OrderID);
                    if (!string.IsNullOrEmpty(formOrderLines))
                        ltFormItems.Text = string.Format("<tr><td colspan='6'>" +
                            "<table class='table table-bordered table-dark ms-3'><thead><th>Form #</th><th>Form Title</th><th>Description</th>" +
                            "<th>Quantity</th><th>End User</th><th>Comments</th><th>Attachments</th></thead><tbody>{0}</tbody>" +
                            "</table></td></tr>", formOrderLines);
                }
                else { ltFormItems.Text = string.Empty; }
            }
        }
        protected string BuildFormOrderLines(int orderId)
        {
            StringBuilder sb = new StringBuilder();
            var ctl = new FormOrderController();
            IEnumerable<FormOrderItem> fi = ctl.GetFormOrderItemsByOrder(orderId);
            foreach (FormOrderItem f in fi)
            {
                string attachments = BuildAttachments(f.FormID);
                attachments = attachments == string.Empty ? "" : string.Format("<ul class='list-unstyled mb-0'>{0}</ul>", attachments);
                sb.Append(string.Format("<tr class='table-secondary'><td>{1}</td><td>{2}</td>" +
                    "<td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>",
                    EditUrl("oid", f.OrderID.ToString(), "detail"), f.FormNumber, f.FormName, f.Description, f.Quantity, 
                    f.Recipient, f.Comments, attachments));
            }
            return sb.ToString();
        }
        protected string BuildAttachments(int formId)
        {
            string attachementList = string.Empty;
            var aCtl = new AttachmentController();
            IEnumerable<FormOrderAttachment> attachments = aCtl.GetFormAttachmentsByFormId(formId);
            FileManager objFile = new FileManager();
            int attachmentCount = 0;
            foreach (FormOrderAttachment f in attachments)
            {
                var file = objFile.GetFile(f.FileID);
                if (file != null)
                    attachementList += string.Format("<li><a href='/portals/0/{0}' title='{1}'>attachment #{2}</a></li>", file.RelativePath,file.FileName, ++attachmentCount);
            }
            return attachementList;
        }
        protected void chkShowCompleted_CheckedChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}