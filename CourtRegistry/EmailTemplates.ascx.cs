/*
' Copyright (c) 2026  12th Judicial Circuit Court
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    /// <summary>
    /// Lets court administration edit the emails the registry sends to an attorney
    /// during the removal process. The wording is revised from time to time, so it
    /// lives in the database rather than in the code.
    ///
    /// Nothing here can stop an email going out: a template that is switched off or
    /// left empty falls back to the wording built into the module.
    /// </summary>
    public partial class EmailTemplates : CourtRegistryModuleBase
    {
        private readonly IHostSettings _hostSettings;

        public EmailTemplates()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        private bool IsApprover
        {
            get
            {
                if (string.IsNullOrEmpty(ApproverUsername) || UserId <= 0)
                    return false;
                return UserInfo.Username.IndexOf(ApproverUsername, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        #region Event Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BindList();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptTemplates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "edit")
            {
                return;
            }
            int templateId;
            if (!int.TryParse(e.CommandArgument.ToString(), out templateId) || templateId <= 0)
            {
                return;
            }
            ShowTemplate(templateId);
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }
            if (!IsApprover)
            {
                Notify("You do not have rights to change these emails.", "error");
                return;
            }

            int templateId;
            if (!int.TryParse(hdTemplateId.Value, out templateId) || templateId <= 0)
            {
                return;
            }

            var ctl = new EmailTemplateController(_hostSettings);
            EmailTemplate template = ctl.GetTemplate(templateId);
            if (template == null)
            {
                Notify("That template could no longer be found.", "error");
                ShowList();
                return;
            }

            DotNetNuke.UI.UserControls.TextEditor editor = BodyEditor;
            string body = editor == null ? string.Empty : editor.RichText.Text;
            if (string.IsNullOrWhiteSpace(StripHtml(body)))
            {
                Notify("The body is empty. Add some wording, or untick the status box to send the built-in email instead.", "warning");
                return;
            }

            template.Subject = txtSubject.Text.Trim();
            template.Description = txtDescription.Text.Trim();
            template.Body = body;
            template.IsEnabled = chkEnabled.Checked;

            ctl.UpdateTemplate(template, UserId);

            Notify(template.TemplateName + " saved.", "success");
            ShowList();
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            ShowList();
        }
        #endregion

        #region Binding helpers
        /// <summary>Shown in the grid; the repeater calls it, so it has to be public.</summary>
        public string EnabledBadge(object isEnabled)
        {
            bool on = isEnabled != null && Convert.ToBoolean(isEnabled);
            return on
                ? "<span class=\"badge bg-success\">In use</span>"
                : "<span class=\"badge bg-secondary\">Built-in wording</span>";
        }

        public string ModifiedText(object dateModified)
        {
            if (dateModified == null || dateModified == DBNull.Value)
            {
                return "<span class=\"text-muted\">Never edited</span>";
            }
            return Convert.ToDateTime(dateModified).ToString("g");
        }
        #endregion

        #region Methods
        /// <summary>
        /// The DNN rich text editor is a user control, so it is reached through
        /// FindControl rather than the generated designer field.
        /// </summary>
        private DotNetNuke.UI.UserControls.TextEditor BodyEditor
        {
            get { return FindControl("txtBody") as DotNetNuke.UI.UserControls.TextEditor; }
        }

        private void BindList()
        {
            var ctl = new EmailTemplateController(_hostSettings);
            List<EmailTemplate> templates;
            try
            {
                templates = ctl.GetTemplates().ToList();
            }
            catch (Exception)
            {
                // The table is not there yet. The emails still send their built-in wording.
                templates = new List<EmailTemplate>();
            }

            rptTemplates.DataSource = templates;
            rptTemplates.DataBind();
            pnlEmpty.Visible = templates.Count == 0;
        }

        private void ShowList()
        {
            pnlEdit.Visible = false;
            pnlList.Visible = true;
            hdTemplateId.Value = string.Empty;
            BindList();
        }

        private void ShowTemplate(int templateId)
        {
            var ctl = new EmailTemplateController(_hostSettings);
            EmailTemplate template = ctl.GetTemplate(templateId);
            if (template == null)
            {
                Notify("That template could no longer be found.", "error");
                ShowList();
                return;
            }

            hdTemplateId.Value = template.TemplateId.ToString();
            ltEditTitle.Text = HttpUtility.HtmlEncode(template.TemplateName);
            txtSubject.Text = template.Subject;
            txtDescription.Text = template.Description;
            chkEnabled.Checked = template.IsEnabled;
            ltTokens.Text = BuildTokenHelp(template.TemplateKey);

            DotNetNuke.UI.UserControls.TextEditor editor = BodyEditor;
            if (editor != null)
            {
                editor.Text = template.Body;
            }

            cmdSave.Enabled = IsApprover;
            if (!IsApprover)
            {
                Notify("You can read these emails but not change them.", "warning");
            }

            pnlList.Visible = false;
            pnlEdit.Visible = true;
        }

        /// <summary>
        /// The placeholders each email can use. Kept beside the sending code's token
        /// bag so the two do not drift apart.
        /// </summary>
        private static string BuildTokenHelp(string templateKey)
        {
            var tokens = new List<string>
            {
                "FullName|the attorney's first and last name",
                "FirstName|first name only",
                "LastName|last name only",
                "BarNumber|their Florida Bar number"
            };

            if (templateKey == EmailTemplateKeys.WithdrawalSubmitted)
            {
                tokens.Add("DateRequested|when they submitted the request");
                tokens.Add("Signature|the name they typed as their signature");
                tokens.Add("Reason|the reason they gave, blank when they gave none");
                tokens.Add("ApplicationId|their current application number");
                tokens.Add("FiscalYear|the fiscal year of that application, e.g. 2026 - 2027");
            }
            else
            {
                tokens.Add("DateReviewed|when court administration acted on it");
                tokens.Add("ReviewNote|the note typed when reviewing, on its own");
                tokens.Add("NoteBlock|that note wrapped in a labelled paragraph, or nothing when none was typed");
            }

            if (templateKey == EmailTemplateKeys.WithdrawalApproved)
            {
                tokens.Add("EffectiveDate|the date the removal takes effect");
            }

            var sb = new StringBuilder();
            sb.Append("<p class=\"mb-0 mt-2\">Wrap an optional section in <code>[if:Token]</code> &hellip; <code>[/if]</code> ");
            sb.Append("and it is left out entirely when that token has no value, instead of leaving a heading with nothing under it.</p>");
            sb.Append("<ul class=\"mb-0 mt-2\">");
            foreach (string token in tokens)
            {
                string[] parts = token.Split('|');
                sb.AppendFormat("<li><code>[{0}]</code> &mdash; {1}</li>",
                    parts[0], HttpUtility.HtmlEncode(parts[1]));
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }
            string text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", string.Empty);
            return HttpUtility.HtmlDecode(text).Replace(" ", " ").Trim();
        }

        private void Notify(string message, string type)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                "new Noty({ text: '" + HttpUtility.JavaScriptStringEncode(message) + "', type: '" + type + "', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
        }
        #endregion
    }
}
