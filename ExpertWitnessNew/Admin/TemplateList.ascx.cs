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
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.ExpertWitness.Components;
namespace tjc.Modules.ExpertWitness
{
    public partial class TemplateList : ExpertWitnessModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public TemplateList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new TemplateController();

            rptTemplate.DataSource = ctl.GetTemplates();
            rptTemplate.DataBind();
        }
        private void ClearForm()
        {
            hdTemplateId.Value = string.Empty;
            hdRequirements.Value = string.Empty;
            txtTemplateName.Text = string.Empty;
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
                    var ctl = new TypeController();
                    clsTemplateRequirements.DataTextField = "TypeName";
                    clsTemplateRequirements.DataValueField = "TypeID";
                    clsTemplateRequirements.DataSource = ctl.GetTypes();
                    clsTemplateRequirements.DataBind();
                    List<TemplateExpertSelection> lstTemplateExpertSelections = new List<TemplateExpertSelection>();
                    rptTemplateTypes.DataSource = lstTemplateExpertSelections;
                    rptTemplateTypes.DataBind();
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
            var ctl = new TemplateController();
           Template template = new Template();
            bool isNew = true;
            if (hdTemplateId.Value != "")
            {
                isNew = false;
                template = ctl.GetTemplate(Convert.ToInt32(hdTemplateId.Value));
            }
            template.TemplateName = txtTemplateName.Text;
            template.ModifiedDate = DateTime.Now;
            template.ModifiedBy = UserInfo.Username;
            if (isNew)
            {
                template.CreatedBy = UserInfo.Username;
                template.CreatedDate = DateTime.Now;
                ctl.CreateTemplate(template);
            }
            else
            {
                ctl.DeleteTemplateTypes(template.TemplateID);
                ctl.DeleteTemplateSequences(template.TemplateID);
                ctl.UpdateTemplate(template);
            }
            string json = hdRequirements.Value;
            if (!string.IsNullOrEmpty(json))
            {
                var serializer = new JavaScriptSerializer();
                var templateRequirements = serializer.Deserialize<List<TemplateRequirement>>(json);
                foreach (TemplateRequirement requirement in templateRequirements)
                {
                    int seq = requirement.Sequence;
                    ctl.CreateTemplateSequence(template.TemplateID, seq, requirement.NumberRequired);
                    foreach (Components.Type type in requirement.Types)
                    {
                        ctl.CreatTemplateType(template.TemplateID,type.TypeID,seq);
                    }
                }
            }
            ClearForm();
            BindList();
        }
        protected void pnlTemplates_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptTemplate_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int templateId = Convert.ToInt32(e.CommandArgument);
            var ctl = new TemplateController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteTemplate(templateId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Components.Template template = ctl.GetTemplate(templateId);
                List<TemplateExpertSelection> lstTemplateExpertSelections = new List<TemplateExpertSelection>();
                IEnumerable<TemplateSequence> templateSequences = ctl.GetTemplateSequences(templateId);
                foreach (TemplateSequence templateSequence in templateSequences)
                {
                    TemplateExpertSelection templateExpertSelection = new TemplateExpertSelection();
                    templateExpertSelection.Sequence = templateSequence.Sequence;
                    templateExpertSelection.NumberRequired = templateSequence.NumberRequired;
                    templateExpertSelection.ExpertTypes = ctl.GetTemplateTypeTypesBySequence( templateId,templateSequence.Sequence);
                    lstTemplateExpertSelections.Add(templateExpertSelection);
                }
                rptTemplateTypes.DataSource = lstTemplateExpertSelections.OrderBy(x => x.Sequence);
                rptTemplateTypes.DataBind();

                hdTemplateId.Value = templateId.ToString();
                hdRequirements.Value=string.Empty;
                txtTemplateName.Text = template.TemplateName;
                ScriptManager.RegisterStartupScript(rptTemplate, rptTemplate.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptTemplate_ItemCreated(object sender, RepeaterItemEventArgs e)
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