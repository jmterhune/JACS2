/*
' Copyright (c) 2023  12th Judicial Circuit
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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.ExpertWitness.Components;
namespace tjc.Modules.ExpertWitness
{
    public partial class ExpertList : ExpertWitnessModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public ExpertList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new ExpertController();
            rptExpert.DataSource = ctl.GetExperts();
            rptExpert.DataBind();
        }
        private void PopulateCheckboxLists(int expertId)
        {
            var lCtl =new ExpertController();
            IEnumerable<Location> locations=lCtl.GetExpertLocationLocations(expertId);
            IEnumerable<Components.Type> types = lCtl.GetExpertTypeTypes(expertId);
            IEnumerable<Template> templates = lCtl.GetExpertTemplateTemplates(expertId);
            foreach (Location location in locations)
            {
                ListItem li = clsLocations.Items.FindByValue(location.LocationID.ToString());
                li.Selected = true;
            }
            foreach (Components.Type type in types)
            {
                ListItem li = clsTypes.Items.FindByValue(type.TypeID.ToString());
                li.Selected = true;
            }
            foreach (Template template in templates)
            {
                ListItem li = clsEvaluationTypes.Items.FindByValue(template.TemplateID.ToString());
                li.Selected = true;
            }
        }
        private void InitializeCheckboxLists()
        {
            var lCtl = new LocationController();
            var tCtl = new TypeController();
            var eCtl = new TemplateController();
            clsLocations.DataTextField = "LocationName";
            clsLocations.DataValueField = "LocationID";
            clsLocations.DataSource = lCtl.GetLocations();
            clsLocations.DataBind();
            clsTypes.DataTextField = "TypeName";
            clsTypes.DataValueField = "TypeID";
            clsTypes.DataSource = tCtl.GetTypes();
            clsTypes.DataBind();
            clsEvaluationTypes.DataTextField = "TemplateName";
            clsEvaluationTypes.DataValueField = "TemplateID";
            clsEvaluationTypes.DataSource = eCtl.GetTemplates();
            clsEvaluationTypes.DataBind();
        }
        private void ClearForm()
        {
            hdExpertId.Value = string.Empty;
            txtExpertName.Text = string.Empty;
            txtComments.Text = string.Empty;
            txtContractEnds.Text = string.Empty;
            foreach (ListItem li in clsLocations.Items)
            {
                li.Selected = false;
            }
            foreach (ListItem li in clsTypes.Items)
            {
                li.Selected = false;
            }
            foreach (ListItem li in clsEvaluationTypes.Items)
            {
                li.Selected = false;
            }
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    InitializeCheckboxLists();
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new ExpertController();
            Components.Expert expert = new Components.Expert();
            bool isNew = true;
            if (hdExpertId.Value != "")
            {
                isNew = false;
                expert = ctl.GetExpert(Convert.ToInt32(hdExpertId.Value));
                ctl.DeleteExpertLocations(expert.ExpertID);
                ctl.DeleteExpertTemplates(expert.ExpertID);
                ctl.DeleteExpertTypes(expert.ExpertID);
            }
            expert.Description = txtExpertName.Text;
            if (!string.IsNullOrEmpty(txtContractEnds.Text))
            {
                DateTime.TryParse(txtContractEnds.Text, out DateTime contractEnd);
                expert.ContractEnds = contractEnd;
            }
            expert.ModifiedDate = DateTime.Now;
            expert.ModifiedBy = UserInfo.Username;
            expert.Comments = txtComments.Text;
            if (isNew)
            {
                expert.CreatedBy = UserInfo.Username;
                expert.CreatedDate = DateTime.Now;
                ctl.CreateExpert(expert);
            }
            else
            {
                ctl.UpdateExpert(expert);
            }
            foreach (ListItem item in clsLocations.Items)
            {
                if (item.Selected)
                    ctl.CreateExpertLocation(expert.ExpertID, Int32.Parse(item.Value));
            }
            foreach (ListItem item in clsEvaluationTypes.Items)
            {
                if (item.Selected)
                    ctl.CreateExpertTemplate(expert.ExpertID, Int32.Parse(item.Value));
            }
            foreach (ListItem item in clsTypes.Items)
            {
                if (item.Selected)
                    ctl.CreateExpertType(expert.ExpertID, Int32.Parse(item.Value));
            }
            ClearForm();
            BindList();
        }
        protected void pnlExperts_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptExpert_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int expertId = Convert.ToInt32(e.CommandArgument);
            var ctl = new ExpertController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteExpert(expertId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                ClearForm();
                Components.Expert expert = ctl.GetExpert(expertId);
                hdExpertId.Value = expertId.ToString();
                txtExpertName.Text = expert.Description;
                PopulateCheckboxLists(expertId);
                if(expert.ContractEnds.HasValue)
                txtContractEnds.Text=expert.ContractEnds.Value.ToShortDateString();
                txtComments.Text = expert.Comments;
                ScriptManager.RegisterStartupScript(rptExpert, rptExpert.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptExpert_ItemCreated(object sender, RepeaterItemEventArgs e)
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
    }
}