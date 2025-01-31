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
using System.Collections.Generic;
using System.Linq;
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
    public partial class Case : ProSeLogModuleBase
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
                    }
                    var ctl = new HistoryController();
                    IEnumerable<HistoryListItem> histories = ctl.GetHistoryListItemsByCaseNumber(CaseNumber, 0);
                    HistoryListItem history = histories.FirstOrDefault();
                    if (history != null)
                    {
                        txtCaseName.Text = history.CaseName;
                        txtCaseNumber.Text = history.CaseNumber.ToUpper();
                        txtCaseTypeName.Text = history.CaseTypeName;
                        txtPetitioner.Text = history.Petitioner;
                        txtRespondent.Text = history.Respondent;
                        lnkNewProject.NavigateUrl = EditUrl("hid", history.HistoryID.ToString(), "form", "copy=1");
                    }
                    rptHistoryList.DataSource = histories;
                    rptHistoryList.DataBind();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion
    }
}