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
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing
{
    public partial class ViewStamp : PurchasingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public ViewStamp()
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
                    txtStartDate.Text = DateTime.Now.AddDays(-30).ToShortDateString();
                    txtEndDate.Text = DateTime.Now.ToShortDateString();
                    BindData();
                    lnkReset.NavigateUrl = EditUrl("list");
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
            var ctl = new StampOrderController();
            var orders = ctl.GetOrders(startDate, endDate.AddDays(1));
            if (chkShowCompleted.Checked)
            {
                rptOrders.DataSource = orders;
            }
            else
            {
                rptOrders.DataSource = orders.Where(x => x.CompletedDate == null);
            }
            rptOrders.DataSource = orders;
            rptOrders.DataBind();
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int orderId = int.Parse(e.CommandArgument.ToString());
            var ctl = new StampOrderController();

            if (e.CommandName == "toggle")
            {
                var objOrder = ctl.GetStampOrder(orderId);
                if (objOrder != null)
                {
                    if (objOrder.CompletedDate != null)
                    {
                        objOrder.CompletedDate = null;
                        objOrder.Status = OrderStatus.@new;
                    }
                    else
                    {
                        objOrder.CompletedDate = DateTime.Now;
                        objOrder.Status = OrderStatus.completed;
                    }
                    ctl.UpdateStampOrder(objOrder);
                }
            }
            if (e.CommandName == "delete")
            {
                ctl.DeleteStampOrder(orderId);
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
                Components.StampOrder item = (Components.StampOrder)e.Item.DataItem;
                HyperLink lnkDetails = (HyperLink)e.Item.FindControl("lnkDetails");
                lnkDetails.NavigateUrl = EditUrl("oid", item.OrderID.ToString(), "detail");
            }
        }

        protected void chkShowCompleted_CheckedChanged(object sender, EventArgs e)
        {
            BindData();
        }
    }
}