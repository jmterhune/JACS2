/*
' Copyright (c) 2019  jud12
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from ThreatReportModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Edit : ThreatReportModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Edit()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        private List<Person> PersonList
        {
            get
            {
                object value = HttpContext.Current.Session["PersonList"];
                if (value != null)
                {
                    return (List<Person>)value;
                }
                return new List<Person>();
            }
            set
            {
                HttpContext.Current.Session["PersonList"] = value;
            }
        }
        protected void BindPersonList()
        {
            rptPersonsInvolved.DataSource = PersonList;
            rptPersonsInvolved.DataBind();
            updatePersonsInvolved.Update();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    
                    txtDate.Text = DateTime.Today.ToString("MM/dd/yyyy");
                    PersonList = null;
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    rptPersonsInvolved.DataSource = PersonList;
                    rptPersonsInvolved.DataBind();
                    var t = new Incident();
                    var tc = new IncidentController();

                    t = new Incident()
                    {
                        CreatedByUserID = UserId,
                        DateOfIncident = DateTime.Now,
                        DateReported=DateTime.Now,
                        DateReportedLEO=DateTime.Now,
                        

                    };
                    tc.CreateIncident(t);
                    hdIncidentId.Value = t.IncidentID.ToString();
                    string physicalDirectory = "C:\\websites\\Threat\\Attachments";
                    if (Settings.Contains("AttachmentDirectory"))
                    {
                        physicalDirectory = Settings["AttachmentDirectory"].ToString();
                    }
                    DirectoryInfo dir = new DirectoryInfo(physicalDirectory);
                    if (!dir.Exists)
                    {
                        try
                        {
                            dir.Create();
                        }
                        catch { }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var t = new Incident();
                var tc = new IncidentController();
                var pc = new PersonController();
                DateTime incidentDate = DateTime.Now;
                DateTime reportedDate = DateTime.Now;
                DateTime reportedLeoDate = DateTime.Now;
                if (txtDate.Text != "")
                {
                    DateTime.TryParse(txtDate.Text, out incidentDate);
                }
                if (txtDateReported.Text != "")
                {
                    DateTime.TryParse(txtDateReported.Text, out reportedDate);

                }
                if (txtDateReportedLeo.Text != "")
                {
                    DateTime.TryParse(txtDateReportedLeo.Text, out reportedLeoDate);

                }
                t = tc.GetIncident(Int32.Parse(hdIncidentId.Value));
                t.Location = rblLocation.SelectedValue;
                t.ReportedBy = txtPersonReporting.Text.Trim();
                t.NatureOfIncident = rblIncidentNature.SelectedValue;
                t.Description = txtIncidentDescription.Text.Trim();
                t.PersonTargeted = txtPersonTargeted.Text.Trim();
                t.ActionTaken = txtActionTaken.Text.Trim();
                t.IsCourtEmployee = chkCourtEmployee.Checked;
                t.WasTargetNotified = chkTargetNotified.Checked;
                t.CreatedByUserID = UserId;
                t.DateOfIncident = incidentDate;
                t.ReporterPhone = txtPersonReportingPhone.Text.Trim();
                t.ReporterExt = txtPersonReportingExtension.Text.Trim();
                t.ReporterEmail = txtPersonReportingEmail.Text.Trim();
                t.DateReported = reportedDate;
                t.DateReportedLEO = reportedLeoDate;
                t.CaseNumber = txtCaseNumber.Text.ToUpper().Trim();
                t.PersonReportingToLEO = txtPersonReportingLeo.Text.Trim();
                t.LEOAgency = txtAgency.Text.Trim();
                tc.UpdateIncident(t);
                var list = PersonList;
                foreach (Person person in list)
                {
                    person.IncidentID = t.IncidentID;
                    pc.CreatePerson(person);
                }
                PersonList = null;
                SendEmails(t, list);
                Response.Redirect(EditUrl("id", t.IncidentID.ToString(), "complete"));

            }
            catch (Exception exc)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "An error occurred subitting your requrest.  Please report this error to the <a href='/Contact-Us?eid=d2ViaGVscA%3d%3d-E%2fjiw0ctjEE%3d'>Help Desk</a>", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void SendEmails(Incident incident, List<Person> people)
        {
            string message = "";
            string subject = "12th Judicial Circuit Incident Report";
            string from = "noreply.threat@jud12.flcourts.org";
            string viewerRole = "Incident Reporters";
            if (Settings.Contains("ViewerRole"))
            {
                viewerRole = Settings["ViewerRole"].ToString();
            }
            string href = EditUrl("id", incident.IncidentID.ToString(), "incident");
            StringBuilder sb = new StringBuilder();
            if (people.Count > 0)
            {
                sb.Append("An incident report has been filed at ").Append(incident.Location).Append(" on the following person(s)");
                sb.Append(Environment.NewLine);
                foreach (var person in people)
                {
                    sb.Append("\t").Append("-").Append(person.FirstName);
                    sb.Append(" ").Append(person.LastName).Append(Environment.NewLine);
                }
                sb.Append(Environment.NewLine);
                sb.Append("You may access details at the following URL: ");
                sb.Append(Environment.NewLine);
                sb.Append(href);
            }
            else
            {
                sb.Append("An incident report has been filed at ").Append(incident.Location).Append(". You may access detail at the following URL: ");
                sb.Append(href);
            }
            message = sb.ToString();
            tjc.Modules.Globals.Components.SendBulkMail bEmail = new tjc.Modules.Globals.Components.SendBulkMail()
            {
                FromAddress = from,
                Subject = subject,
                Body = message
            };
            DotNetNuke.Security.Roles.RoleController rCtl = new DotNetNuke.Security.Roles.RoleController();
            var emailusers = rCtl.GetUsersByRole(PortalId, viewerRole);
            foreach (var user in emailusers)
            {
                bEmail.AddEmailAddress(user.Email);
            }
            Thread objThreadMail = new Thread(bEmail.Send);
            objThreadMail.Start();

        }

        protected void rptPersonsInvolved_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "delete")
            {
                int id = Int32.Parse(e.CommandArgument.ToString());
                var list = PersonList;
                list.RemoveAt(id);
                PersonList = list;
                BindPersonList();

            }
        }

        protected void rptPersonsInvolved_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);

                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
            }
        }

        protected void cmdSavePerson_Click(object sender, EventArgs e)
        {
            DateTime.TryParse(txtDOB.Text, out DateTime dob);
            if (dob == DotNetNuke.Common.Utilities.Null.NullDate)
            {
                dob = DateTime.Now;
            }
            var list = PersonList;
            var person = new Person()
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                DateOfBirth = dob,
                HairColor = txtHairColor.Text.Trim(),
                Gender = drpGender.SelectedValue,
                Race = drpRace.SelectedValue,
                Height = txtHeight.Text.Trim(),
                Weight = txtWeight.Text.Trim(),
                Features = txtFeatures.Text.Trim(),
                Vehicle = txtVehicle.Text.Trim(),
                Voice = txtVoice.Text.Trim(),
                Phone = txtPhonePerson.Text.Trim(),
            };
            list.Add(person);
            PersonList = list;
            BindPersonList();
        }
        protected void ValidatePersons(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (PersonList.Count > 0)
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                }
            }

            catch (Exception)
            {
                args.IsValid = false;
            }
        }
    }
}