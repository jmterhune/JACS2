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
    public partial class PublicAudioRequest : AudioRequestModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        public string ClientKey = WebConfigurationManager.AppSettings["reCaptcha3SiteKey"];
        private readonly IReCaptchaService _reCaptchaService;

        #endregion
        #region Methods
        public PublicAudioRequest()
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
            if (!valEmailReq.IsValid)
            {
                errors += valEmailReq.ErrorMessage + "</li>";
            }
            if (!this.valCDInfo.IsValid)
            {
                errors += "<li>" + valCDInfo.ErrorMessage + "</li>";
            }
            if (!this.valCounty.IsValid)
            {
                errors += "<li>" + valCounty.ErrorMessage + "</li>";
            }
            if (!this.valCaseName.IsValid)
            {
                errors += "<li>" + valCaseName.ErrorMessage + "</li>";
            }
            if (!this.valCaseNumber.IsValid)
            {
                errors += "<li>" + valCaseNumber.ErrorMessage + "</li>";
            }
            if (!this.valJudge.IsValid)
            {
                errors += "<li>" + valJudge.ErrorMessage + "</li>";
            }
            if (!this.valDates.IsValid)
            {
                errors += "<li>" + valDates.ErrorMessage + "</li>";
            }

            if (!this.valLocation.IsValid)
            {
                errors += "<li>" + valLocation.ErrorMessage + "</li>";
            }
            if (!this.valInvolvement.IsValid)
            {
                errors += "<li>" + valInvolvement.ErrorMessage + "</li>";
            }
            if (!this.valDelivery.IsValid)
            {
                errors += "<li>" + valDelivery.ErrorMessage + "</li>";
            }
            if (!this.valPickup.IsValid)
            {
                errors += "<li>" + valPickup.ErrorMessage + "</li>";
            }
            errors += "</ul></div>";
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + System.Guid.NewGuid().ToString("N"),
                "Swal.fire({ title: 'Error', html: '" + System.Web.HttpUtility.JavaScriptStringEncode(errors) + "', icon: 'error', confirmButtonText: 'OK' });", true);
        }

        private ProceedingInfo AddRecord()
        {
            ProceedingInfo objProceeding = new ProceedingInfo();
            try
            {
                ProceedingController ctl = new ProceedingController();
                string _proType = "";
                objProceeding.ModuleId = ModuleId;
                objProceeding.CDPreference = this.rblCDInfo.SelectedValue;
                objProceeding.CaseName = this.txtCaseName.Text;
                objProceeding.CaseNumber = this.txtCaseNumber.Text;
                objProceeding.ProceedingDate = this.txtDates.Text.Trim();
                if (this.rblInvolvement.SelectedValue == "Other")
                {
                    objProceeding.Involvement = this.txtOther.Text.Trim();
                }
                else
                {
                    objProceeding.Involvement = this.rblInvolvement.SelectedValue.Trim();
                }
                objProceeding.Paid = false;
                objProceeding.Agency = false;
                objProceeding.Closed = false;
                objProceeding.CA = false;
                objProceeding.Judge = this.txtJudge.Text.Trim();
                objProceeding.Jurisdiction = this.rblCounty.SelectedValue.Trim();
                objProceeding.Location = this.rblLocation.SelectedValue.Trim();
                objProceeding.Instructions = this.txtComment.Text.Trim();
                objProceeding.ProceedingTime = this.txtTime.Text.Trim();
                objProceeding.RequestedDate = DateTime.Now;
                if (this.cklType.Items.Count > 0)
                {
                    foreach (ListItem lstItem in cklType.Items)
                    {
                        if (lstItem.Selected)
                        {
                            _proType += lstItem.Value + ", ";
                        }
                    }
                    objProceeding.ProceedingType = _proType.Remove(_proType.LastIndexOf(","), 1).Trim();
                }
                objProceeding.Address = this.txtAddress.Text.Trim();
                objProceeding.City = this.txtCity.Text.Trim();
                objProceeding.Email = this.txtEMail.Text.Trim();
                objProceeding.IsInquiry = false;
                objProceeding.Requestor = this.txtReqName.Text.Trim();
                objProceeding.Phone = this.txtPhone.Text.Trim();
                objProceeding.State = this.txtState.Text.Trim();
                objProceeding.Zip = this.txtZip.Text.Trim();
                objProceeding.TranscriptionList = this.rblTranscription.SelectedValue;
                if (this.rblDelivery.SelectedValue == "1")
                {
                    objProceeding.DeliveryMethod = this.rblDelivery.SelectedItem.Text + "," + this.rblPickup.SelectedValue;
                }
                else
                {
                    objProceeding.DeliveryMethod = this.rblDelivery.SelectedItem.Text;
                }

                ctl.CreateItem(objProceeding);
                return objProceeding;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                return null;
                // Skins.Skin.AddModuleMessage(Me, "<p><em>Sorry... There was an Error Adding the Audio Request Record.  Please try again.</em><p>If Errors persist please contact <a href=""mailto:courtweb@jud12.flcourts.org"" title=""support"">support</a></p>", Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            }
        }

        private void GetAdobeForm(ProceedingInfo objProceeding)
        {
            try
            {
                StringBuilder urlParam = new StringBuilder();
                urlParam.Append("?jurisdiction=" + Server.UrlEncode(objProceeding.Jurisdiction));
                urlParam.Append("&reqname=" + Server.UrlEncode(objProceeding.Requestor));
                urlParam.Append("&reqfirm=" + Server.UrlEncode(txtFirm.Text.ToString()));
                urlParam.Append("&reqaddress=" + Server.UrlEncode(objProceeding.Address));
                urlParam.Append("&reqcity=" + Server.UrlEncode(objProceeding.City + ", " + objProceeding.State + "  " + objProceeding.Zip));
                urlParam.Append("&reqphone=" + Server.UrlEncode(objProceeding.Phone));
                urlParam.Append("&reqemail=" + Server.UrlEncode(objProceeding.Email));
                urlParam.Append("&casename=" + Server.UrlEncode(objProceeding.CaseName));
                urlParam.Append("&casenumber=" + Server.UrlEncode(objProceeding.CaseNumber));
                urlParam.Append("&judge=" + Server.UrlEncode(objProceeding.Judge));
                urlParam.Append("&procdate=" + Server.UrlEncode(objProceeding.ProceedingDate));
                urlParam.Append("&proctime=" + Server.UrlEncode(objProceeding.ProceedingTime));
                urlParam.Append("&proclocation=" + Server.UrlEncode(objProceeding.Location));
                urlParam.Append("&proctype=" + Server.UrlEncode(objProceeding.ProceedingType));
                urlParam.Append("&involvement=" + Server.UrlEncode(objProceeding.Involvement));
                urlParam.Append("&delivery=" + Server.UrlEncode(objProceeding.DeliveryMethod));
                urlParam.Append("&transcrip=" + Server.UrlEncode(objProceeding.TranscriptionList));
                urlParam.Append("&cdtype=" + Server.UrlEncode(objProceeding.CDPreference));

                string url = Page.ResolveUrl(this.TemplateSourceDirectory + "/pdfForm.aspx") + urlParam.ToString();
                Response.Redirect(url, true);
            }
            catch
            {
            }
        }

        private void SendEmail()
        {
            try
            {
                string emailAddress = this.txtEMail.Text.Trim();
                string body = "This is confirmation of your Audio Request. <br />Before your request can be processed, payment must be received <br />by the Digital Recording Office.";
                string subject = "Audio Request Confirmation";
                // DotNetNuke.Services.Mail.Mail.SendEmail("noreply@jud12.flcourts.org", emailAddress, subject, body);
                AsyncEmails("noreply.dcr@jud12.flcourts.org", emailAddress, subject, body);
            }
            catch
            {
            }
        }

        private void AsyncEmails(string from, string to, string subject, string message)
        {
            try
            {
                Common.Utilities.Mail.SendBulkMail bMail = new Common.Utilities.Mail.SendBulkMail();
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

        public void ValidatePickup(object source, ServerValidateEventArgs args)
        {
            bool result = false;
            if (this.rblDelivery.SelectedIndex == 0)
            {
                if (this.rblPickup.SelectedIndex < 0)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }
            args.IsValid = result;
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
                ProceedingInfo objproceeding = AddRecord();
                SendEmail();
                GetAdobeForm(objproceeding);
            }
            else
            {
                SetValidationErrors();
            }

        }
        #endregion
    }
}