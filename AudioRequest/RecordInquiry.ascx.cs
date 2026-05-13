/*
' Copyright (c) 2017  12th Judicial Circuit
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using tjc.Modules.AudioRequest.Components;

namespace tjc.Modules.AudioRequest
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from AudioRequestModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class RecordInquiry : AudioRequestModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        public string ClientKey = WebConfigurationManager.AppSettings["reCaptcha3SiteKey"];
        private readonly IReCaptchaService _reCaptchaService;
        #endregion
        #region Methods
        public RecordInquiry()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _reCaptchaService = DependencyProvider.GetRequiredService<IReCaptchaService>();

        }
        private void SetValidationErrors()
        {
            string errors = "";
            errors = "<div class=\"errors\"><p><em>Sorry... The data you submitted could not be validated.<br /> Please correct the errors listed below and try again:</em></p><ul>";
            if (!this.valReqName.IsValid)
            {
                errors += "<li>" + valReqName.ErrorMessage + "</li>";
            }
            if (!valAddress.IsValid)
            {
                errors += "<li>" + valAddress.ErrorMessage + "</li>";
            }
            if (!this.valCity.IsValid)
            {
                errors += "<li>" + valCity.ErrorMessage + "</li>";
            }
            if (!this.valState.IsValid)
            {
                errors += "<li>" + valState.ErrorMessage + "</li>";
            }
            if (!this.valIsZip.IsValid)
            {
                errors += "<li>" + valIsZip.ErrorMessage + "</li>";
            }
            if (!this.valZip.IsValid)
            {
                errors += "<li>" + valZip.ErrorMessage + "</li>";
            }
            if (!valPhone.IsValid)
            {
                errors += "<li>" + valPhone.ErrorMessage + "</li>";
            }
            if (!this.valIsPhone.IsValid)
            {
                errors += "<li>" + valIsPhone.ErrorMessage + "</li>";
            }
            if (!valEmail.IsValid)
            {
                errors += "<li>" + valEmail.ErrorMessage + "</li>";
            }
            if (!this.valJudge.IsValid)
            {
                errors += "<li>" + valJudge.ErrorMessage + "</li>";
            }
            if (!this.valCounty.IsValid)
            {
                errors += "<li>" + valCounty.ErrorMessage + "</li>";
            }
            if (!this.valCaseNumber.IsValid)
            {
                errors += "<li>" + valCaseNumber.ErrorMessage + "</li>";
            }

            if (!this.valDates.IsValid)
            {
                errors += "<li>" + valDates.ErrorMessage + "</li>";
            }
            //if (!this.valTime.IsValid)
            //{
            //    errors += "<li>" + valTime.ErrorMessage + "</li>";
            //}
            //if (!this.valTimeFormat.IsValid)
            //{
            //    errors += "<li>" + valTimeFormat.ErrorMessage + "</li>";
            //}

            errors += "</ul></div>";
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + System.Guid.NewGuid().ToString("N"),
                "Swal.fire({ title: 'Error', html: '" + System.Web.HttpUtility.JavaScriptStringEncode(errors) + "', icon: 'error', confirmButtonText: 'OK' });", true);
        }

        private void AddRecord()
        {
            try
            {
                ProceedingInfo objProceeding = new ProceedingInfo();
                ProceedingController ctl = new ProceedingController();
                objProceeding.ModuleId = ModuleId;
                objProceeding.CDPreference = "NA";
                objProceeding.CaseNumber = this.txtCaseNumber.Text;
                objProceeding.ProceedingDate = this.txtDates.Text.Trim();
                objProceeding.Judge = this.txtJudge.Text.Trim();
                objProceeding.Jurisdiction = this.rblCounty.SelectedValue.Trim();
                objProceeding.ProceedingTime = this.txtTime.Text.Trim();
                objProceeding.Paid = false;
                objProceeding.Agency = false;
                objProceeding.Closed = false;
                objProceeding.CA = false;
                objProceeding.CaseName = this.txtDefendant.Text.Trim();
                objProceeding.Location = this.rblCounty.SelectedValue.Trim();
                objProceeding.Involvement = "NA";
                objProceeding.ProceedingType = "Criminal Felony";
                objProceeding.Address = this.txtReqAddress.Text.Trim();
                objProceeding.City = this.txtCity.Text.Trim();
                objProceeding.Email = this.txtEMail.Text.Trim();
                objProceeding.Requestor = this.txtReqName.Text.Trim();
                objProceeding.Phone = this.txtPhone.Text.Trim();
                objProceeding.State = this.txtState.Text.Trim();
                objProceeding.Zip = this.txtZip.Text.Trim();
                objProceeding.RequestedDate = DateTime.Now;
                objProceeding.IsInquiry = true;
                ctl.CreateItem(objProceeding);
                SendEmails(objProceeding);
                Response.Redirect(EditUrl("inquiry", "true", "Complete"), true);
            }
            catch (Exception exc)
            {
                string errors = "<p><em>Sorry... There was an Error Adding the Request Record.  Please try again.</em><p>If Errors persist please contact <a href=\"mailto:courtweb@jud12.flcourts.org\" title=\"support\">support</a></p>";
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + System.Guid.NewGuid().ToString("N"),
                    "Swal.fire({ title: 'Error', html: '" + System.Web.HttpUtility.JavaScriptStringEncode(errors) + "', icon: 'error', confirmButtonText: 'OK' });", true);
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void SendEmails(ProceedingInfo objProceeding)
        {
            string mailTo = "";
            string secondaryMailTo = "";
            if (Settings.Contains("Email"))
            {
                mailTo = Settings["Email"].ToString().Trim();
            }
            if (Settings.Contains("Email2"))
            {
                secondaryMailTo = Settings["Email2"].ToString().Trim();
            }
            if (objProceeding.Jurisdiction.ToLower() == "manatee" | objProceeding.Jurisdiction.ToLower() == "desoto")
            {
                if (mailTo != "")
                {
                    //    DotNetNuke.Services.Mail.Mail.SendEmail(objProceeding.Email, mailTo, objProceeding.Jurisdiction + "Audio Request", EmailBody(objProceeding));
                    AsyncEmails(objProceeding.Email, mailTo, objProceeding.Jurisdiction + "Inquiry", EmailBody(objProceeding));
                }
            }
            else
            {
                if (secondaryMailTo != "")
                {
                    // DotNetNuke.Services.Mail.Mail.SendEmail(objProceeding.Email, secondaryMailTo, objProceeding.Jurisdiction + "Audio Request", EmailBody(objProceeding));
                    AsyncEmails(objProceeding.Email, secondaryMailTo, objProceeding.Jurisdiction + "Inquiry", EmailBody(objProceeding));

                }
            }
            //    DotNetNuke.Services.Mail.Mail.SendEmail("noreply@jud12.flcourts.org", mailTo, objProceeding.Jurisdiction + "Audio Request", EmailBody(objProceeding));
            //AsyncEmails("noreply@jud12.flcourts.org", mailTo, objProceeding.Jurisdiction + "Inquiry", EmailBody(objProceeding));
        }

        private void AsyncEmails(string from, string to, string subject, string message)
        {
            try
            {
                tjc.Common.Utilities.Mail.SendBulkMail bMail = new tjc.Common.Utilities.Mail.SendBulkMail();
                bMail.FromAddress = from;
                bMail.AddEmailAddress(to);
                bMail.Subject = subject;
                bMail.Body = message;
                Thread objThread = new Thread(bMail.Send);
                objThread.Start();

            }
            catch
            {
            }
        }

        private string EmailBody(ProceedingInfo objProceeding)
        {
            StringBuilder message = new StringBuilder();
            message.Append("<b>Court Proceedings Audio Request Form</b> <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Requestor Information</b> <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Representing:</b>" + objProceeding.Involvement + " <br />");
            message.Append("<b>Requestor:</b>" + objProceeding.Requestor + " <br />");
            message.Append("<b>Address:</b>" + objProceeding.Address + " <br />");
            message.Append("<b>City, State Zip:</b>" + objProceeding.City + "," + objProceeding.State + " " + objProceeding.Zip + " <br />");
            message.Append("<b>Phone:</b>" + objProceeding.Phone + " <br />");
            message.Append("<b>Email Address:</b>" + objProceeding.Email + " <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Proceeding Information</b> <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Case Name:</b>" + objProceeding.CaseName + " <br />");
            message.Append("<b>Case Number:</b>" + objProceeding.CaseNumber + " <br />");
            message.Append("<b>Presiding Judge:</b>" + objProceeding.Judge + " <br />");
            message.Append("<b>Date of Proceeding:</b>" + objProceeding.ProceedingDate + " <br />");
            message.Append("<b>Time of Proceeding:</b>" + objProceeding.ProceedingTime + " <br />");
            message.Append("<b>Location of Proceeding:</b>" + objProceeding.Location + " <br />");
            message.Append("<b>Type of Proceeding:</b>" + objProceeding.ProceedingType + " <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Special Instructions</b> <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Special Instructions:</b>" + objProceeding.Instructions + " <br />");
            return message.ToString();
        }

        #endregion


        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string reportingPage = "";
                    if (Settings.Contains("ReportingPage"))
                    {
                        reportingPage = Settings["ReportingPage"].ToString().Trim();
                    }
                    if (reportingPage.Trim() == "")
                    {
                        reportingPage = "/Programs/Court-Reporting-Recording";
                    }
                    if (Casenumber == "") { Context.Response.Redirect(reportingPage); }

                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    if (Casenumber != "")
                    {
                        txtCaseNumber.Text = Casenumber;
                    }
                    if (Email != "")
                    {
                        txtEMail.Text = Email;
                    }

                }
                Literal htmlcode = new Literal();
                string script = $" <script src=\"https://www.google.com/recaptcha/api.js?render={ClientKey}\"></script>";
                htmlcode.Text = script;
                Page.Header.Controls.Add(htmlcode);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            var token = hdreCaptcha.Value;
            if (_reCaptchaService.TokenVerify(token, this.ModuleConfiguration.ModuleTitle) == false)
            {
                ltMessage.Visible = true;
                ltMessage.Text = "<div class='alert alert-danger'><i class='fa fa-warning'></i> Invalid Submission Detected!. Make sure you are not using Internet Explorer.</div>";
                return;
            }
            if (Page.IsValid)
            {
                AddRecord();
            }
            else
            {
                SetValidationErrors();
            }
        }

        #endregion

    }
}