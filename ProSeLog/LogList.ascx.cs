/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Web.UI.WebControls;
using tjc.Modules.ProSeLog.Components;

namespace tjc.Modules.ProSeLog
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ProSeLogModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class LogList : ProSeLogModuleBase
    {
        #region Events


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (IsAdmin)
                    {
                        lnkManage.Visible = true;
                        lnkManage.NavigateUrl = CaseTypeListUrl;
                        var cCtl = new CountyController();
                        drpCounty.DataValueField = "CountyID";
                        drpCounty.DataTextField = "CountyName";
                        drpCounty.DataSource = cCtl.GetCounties();
                        drpCounty.DataBind();
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptHistoryList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HistoryListItem listItem=(HistoryListItem)e.Item.DataItem;
                HyperLink lnkView = (HyperLink)e.Item.FindControl("lnkView");
                HyperLink lnkEdit = (HyperLink)e.Item.FindControl("lnkEdit");
                lnkView.NavigateUrl = EditUrl("case", listItem.CaseNumber, "case-list");
                lnkEdit.NavigateUrl = EditUrl("hid",listItem.HistoryID.ToString(), "form");
            }
        }
        protected void cmdPetitioner_Click(object sender, EventArgs e)
        {
            var ctl = new HistoryController();
            rptHistoryList.DataSource = ctl.GetHistoryListItemsByPetitioner(txtPetitioner.Text, Int32.Parse(drpCounty.SelectedValue));
            rptHistoryList.DataBind();
        }

        protected void cmdRespondent_Click(object sender, EventArgs e)
        {
            var ctl = new HistoryController();
            rptHistoryList.DataSource = ctl.GetHistoryListItemsByRespondent(txtRespondent.Text, Int32.Parse(drpCounty.SelectedValue));
            rptHistoryList.DataBind();

        }

        protected void cmdCaseName_Click(object sender, EventArgs e)
        {
            var ctl = new HistoryController();
            rptHistoryList.DataSource = ctl.GetHistoryListItemsByCaseName(txtCaseName.Text, Int32.Parse(drpCounty.SelectedValue));
            rptHistoryList.DataBind();
        }

        protected void cmdCaseNumber_Click(object sender, EventArgs e)
        {
            var ctl = new HistoryController();
            rptHistoryList.DataSource = ctl.GetHistoryListItemsByCaseNumber(txtCaseNumber.Text, Int32.Parse(drpCounty.SelectedValue));
            rptHistoryList.DataBind();
        }
        #endregion


    }
}