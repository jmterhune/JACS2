/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public partial class UpdateJacCodes : CourtRegistryModuleBase
    {
        private List<CaseType> _caseTypes;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BindCaseTypes();
                    BindList();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindCaseTypes()
        {
            var ctl = new CaseTypeController();
            var caseTypes = ctl.GetCaseTypes().OrderBy(c => c.CaseTypeName).ToList();
            drpCaseType.Items.Clear();
            drpCaseType.Items.Add(new ListItem("-- Select --", ""));
            foreach (var c in caseTypes)
                drpCaseType.Items.Add(new ListItem(c.CaseTypeName, c.CaseTypeID.ToString()));
        }

        private void BindList()
        {
            var ctl = new JacCodeController();
            rptUpdates.DataSource = ctl.GetJacCodeUpdates().OrderBy(u => u.JacCodeID);
            rptUpdates.DataBind();
        }

        public string GetUpdateType(object value)
        {
            if (value == null) return string.Empty;
            int.TryParse(value.ToString(), out int v);
            switch (v)
            {
                case 0: return "New";
                case 1: return "Update";
                case 2: return "Remove";
                default: return string.Empty;
            }
        }

        public string GetCaseTypeName(object caseTypeId)
        {
            if (caseTypeId == null) return string.Empty;
            int.TryParse(caseTypeId.ToString(), out int id);
            if (_caseTypes == null)
            {
                var ctl = new CaseTypeController();
                _caseTypes = ctl.GetCaseTypes().ToList();
            }
            var match = _caseTypes.FirstOrDefault(c => c.CaseTypeID == id);
            return match != null ? match.CaseTypeName : string.Empty;
        }

        protected void rptUpdates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int.TryParse(e.CommandArgument.ToString(), out int jacCodeId);
            var ctl = new JacCodeController();
            if (e.CommandName == "delete" && jacCodeId > 0)
            {
                ctl.DeleteJacCodeUpdate(jacCodeId);
                BindList();
            }
            else if (e.CommandName == "edit" && jacCodeId > 0)
            {
                var update = ctl.GetJacCodeUpdate(jacCodeId);
                if (update != null)
                {
                    BindCaseTypes();
                    hdJacCodeID.Value = update.JacCodeID.ToString();
                    txtJacCodeID.Text = update.JacCodeID.ToString();
                    txtCategory.Text = update.Category;
                    var caseItem = drpCaseType.Items.FindByValue(update.CaseTypeID.ToString());
                    if (caseItem != null) caseItem.Selected = true;
                    var typeItem = drpUpdateType.Items.FindByValue(update.UpdateType.ToString());
                    if (typeItem != null) typeItem.Selected = true;
                    string txtId = txtJacCodeID.ClientID;
                    ltModalScript.Text = "<script>(function(){function s(){var t=document.getElementById('" + txtId + "');if(t)t.readOnly=true;if(typeof bootstrap!=='undefined'&&bootstrap.Modal){bootstrap.Modal.getOrCreateInstance(document.getElementById('updateModal')).show();}else if(typeof jQuery!=='undefined'){jQuery('#updateModal').modal('show');}}if(document.readyState!=='loading'){s();}else{document.addEventListener('DOMContentLoaded',s);}})();</script>";
                }
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new JacCodeController();
            int.TryParse(txtJacCodeID.Text, out int jacCodeId);
            int.TryParse(drpCaseType.SelectedValue, out int caseTypeId);
            int.TryParse(drpUpdateType.SelectedValue, out int updateType);
            if (jacCodeId <= 0) return;

            int.TryParse(hdJacCodeID.Value, out int existingId);
            if (existingId > 0)
            {
                var update = ctl.GetJacCodeUpdate(existingId);
                update.Category = txtCategory.Text.Trim();
                update.CaseTypeID = caseTypeId;
                update.UpdateType = updateType;
                ctl.UpdateJacCodeUpdate(update);
            }
            else
            {
                ctl.CreateJacCodeUpdate(new JacCodeUpdate
                {
                    JacCodeID = jacCodeId,
                    Category = txtCategory.Text.Trim(),
                    CaseTypeID = caseTypeId,
                    UpdateType = updateType
                });
            }
            BindList();
        }

        protected void cmdApply_Click(object sender, EventArgs e)
        {
            var ctl = new JacCodeController();
            var updates = ctl.GetJacCodeUpdates().ToList();
            var errors = new List<string>();

            foreach (var u in updates)
            {
                var jacCode = ctl.GetJacCode(u.JacCodeID);
                try
                {
                    if (u.UpdateType == (int)UpdateType.remove)
                    {
                        if (jacCode != null)
                        {
                            jacCode.Active = false;
                            ctl.UpdateJacCode(jacCode);
                        }
                    }
                    else if (u.UpdateType == (int)UpdateType.@new)
                    {
                        if (jacCode != null)
                        {
                            jacCode.Category = u.Category;
                            jacCode.CaseTypeID = u.CaseTypeID;
                            jacCode.Active = true;
                            ctl.UpdateJacCode(jacCode);
                        }
                        else
                        {
                            ctl.CreateJacCode(new JacCode
                            {
                                JacCodeID = u.JacCodeID,
                                Category = u.Category,
                                CaseTypeID = u.CaseTypeID,
                                Active = true
                            });
                        }
                    }
                    else if (u.UpdateType == (int)UpdateType.update)
                    {
                        if (jacCode != null)
                        {
                            jacCode.Category = u.Category;
                            jacCode.CaseTypeID = u.CaseTypeID;
                            jacCode.Active = true;
                            ctl.UpdateJacCode(jacCode);
                        }
                    }
                    ctl.DeleteJacCodeUpdate(u);
                }
                catch (Exception ex)
                {
                    errors.Add(string.Format("Error processing JAC Code {0}: {1}", u.JacCodeID, ex.Message));
                }
            }

            if (errors.Count > 0)
                ltMessage.Text = string.Format("<div class='alert alert-danger'>{0}</div>", string.Join("<br />", errors));
            else
                ltMessage.Text = "<div class='alert alert-success'><i class='fas fa-thumbs-up'></i>&nbsp;Updates applied successfully.</div>";

            BindList();
        }
    }
}
