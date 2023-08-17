/*
' Copyright (c) 2023  Joe Terhune
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
using tjc.Modules.EmployeeDB.Components;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace tjc.Modules.EmployeeDB
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from EmployeeDBModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class JobClassView : EmployeeDBModuleBase
    {

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }

                    PopulateJobClassList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptJobClasss_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int classId = Convert.ToInt32(e.CommandArgument);
            var ctl = new JobClassController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteJobClass(classId);
                PopulateJobClassList();
            }
            if (e.CommandName == "edit")
            {
                JobClass jobClass = ctl.GetJobClass(classId);

                hdClassId.Value = classId.ToString();
                txtClassName.Text = jobClass.ClassName;
                txtClassCode.Text = jobClass.ClassCode.ToString();
                txtPayGrade.Text = jobClass.PayGrade.ToString();
                txtFLSA.Text=jobClass.FLSA;
                txtEEO.Text=jobClass.EEO.ToString();
                txtMMax.Text = jobClass.MMax.ToString();
                txtMMin.Text = jobClass.MMin.ToString();
                txtAMax.Text = jobClass.AMax.ToString();
                txtAMin.Text = jobClass.AMin.ToString();
                ScriptManager.RegisterStartupScript(rptJobClasss, rptJobClasss.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new JobClassController();
            JobClass jobClass = new JobClass();
            bool isNew = true;
            if (hdClassId.Value != "")
            {
                isNew = false;
                jobClass = ctl.GetJobClass(Convert.ToInt32(hdClassId.Value));
            }
            jobClass.ClassName = txtClassName.Text;
            jobClass.ClassCode =Int32.Parse(txtClassCode.Text);
            jobClass.PayGrade = Int32.Parse(txtPayGrade.Text);
            jobClass.FLSA = txtFLSA.Text;
            jobClass.EEO = Int32.Parse(txtEEO.Text);
            jobClass.MMax = Decimal.Parse(txtMMax.Text);
            jobClass.MMin = Decimal.Parse(txtMMin.Text);
            jobClass.AMax = Decimal.Parse(txtAMax.Text);
            jobClass.AMin = Decimal.Parse(txtAMin.Text);
            jobClass.LastModifiedDate = DateTime.Now;
            jobClass.LastModifiedById = UserId;
            if (isNew)
            {
                jobClass.CreatedById = UserId;
                jobClass.CreatedDate = DateTime.Now;
                ctl.CreateJobClass(jobClass);
            }
            else
            {
                ctl.UpdateJobClass(jobClass);
            }
            ClearForm();
            PopulateJobClassList();
        }
        protected void pnlJobClasss_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptJobClasss_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);

                LinkButton cmdEdit = (LinkButton)e.Item.FindControl("cmdEdit");
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            }
        }

        #endregion

        #region Methods
        private void ClearForm()
        {
            hdClassId.Value = string.Empty;
            txtAMax.Text = string.Empty;
            txtAMin.Text = string.Empty;
            txtClassCode.Text = string.Empty;
            txtClassName.Text = string.Empty;
            txtEEO.Text = string.Empty;
            txtFLSA.Text = string.Empty;
            txtMMax.Text = string.Empty;
            txtMMin.Text = string.Empty;
            txtPayGrade.Text = string.Empty;
        }
        private void PopulateJobClassList()
        {
            var ctl = new JobClassController();
            rptJobClasss.DataSource = ctl.GetJobClasses();
            rptJobClasss.DataBind();
        }
        #endregion

    }
}