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
using System.Web.UI;
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
    public partial class ViewThreat : ThreatReportModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public ViewThreat()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    string href = _navigationManager.NavigateURL();
                    if (Settings.Contains("ViewTabID"))
                    {
                        string setting = Settings["ViewTabID"].ToString();
                        if (setting.Length > 0)
                        {
                            href = setting;
                        }
                    }
                    lnkReturn.NavigateUrl = href;

                    if (!DotNetNuke.Common.Utilities.Null.IsNull(IncidentID))
                    {
                        IncidentController ctlI = new IncidentController();
                        PersonController ctlP = new PersonController();
                        AttachmentController ctlA = new AttachmentController();
                        Incident incident = ctlI.GetIncident(IncidentID);
                        rptPersonsInvolved.DataSource = ctlP.GetPersons(IncidentID);
                        rptPersonsInvolved.DataBind();
                        rptAttachments.DataSource = ctlA.GetAttachments(IncidentID);
                        rptAttachments.DataBind();
                        txtActionTaken.Text = incident.ActionTaken;
                        txtDate.Text = incident.DateOfIncident.ToShortDateString();
                        if (incident.DateReported != DotNetNuke.Common.Utilities.Null.NullDate)
                        {
                            txtDateReported.Text = incident.DateReported.ToShortDateString();
                        }
                        if (incident.DateReportedLEO != DotNetNuke.Common.Utilities.Null.NullDate)
                        {
                            txtDateReportedLeo.Text = incident.DateReportedLEO.ToShortDateString();
                        }
                        txtAgency.Text = incident.LEOAgency;
                        txtCaseNumber.Text = incident.CaseNumber;
                        txtPersonReportingLeo.Text = incident.PersonReportingToLEO;
                        txtPersonReportingEmail.Text = incident.ReporterEmail;
                        txtPersonReportingExtension.Text = incident.ReporterExt;
                        txtPersonReportingPhone.Text = incident.ReporterPhone;
                        txtIncidentDescription.Text = incident.Description;
                        txtPersonReporting.Text = incident.ReportedBy;
                        txtPersonTargeted.Text = incident.PersonTargeted;
                        txtIncidentNature.Text = incident.NatureOfIncident;
                        txtLocation.Text = incident.Location;
                        chkCourtEmployee.Checked = incident.IsCourtEmployee;
                        chkTargetNotified.Checked = incident.WasTargetNotified;
                        IEnumerable<Attachment> attachments = ctlA.GetAttachments(IncidentID);

                    }


                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}