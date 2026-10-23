/*
' Copyright (c) 2021  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.ProcessServer
{
    public class ProcessServerModuleBase : PortalModuleBase
    {
        protected readonly IHostSettings _hostSettings;

        public ProcessServerModuleBase()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        public int PID
        {
            get
            {
                var qs = Request.QueryString["pid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public bool IsHoney
        {
            get
            {
                var qs = Request.QueryString["hp"];
                if (qs != null)
                    return qs.Length > 0;
                return false;
            }
        }

        public int SID
        {
            get
            {
                var qs = Request.QueryString["sid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public string PaymentType
        {
            get
            {
                var qs = Request.QueryString["payment"];
                if (qs != null)
                    return qs;
                return "";
            }

        }
        public bool IsNew
        {
            get
            {
                var qs = Request.QueryString["new"];
                if (qs != null)
                    return qs == "1";
                return false;
            }

        }
       
        public string AdminRole
        {
            get
            {
                if ((Settings.Contains("AdminRole")))
                    return Settings["AdminRole"].ToString();
                return "Process Server Admin";
            }
        }
        public string ChiefJudgeRole
        {
            get
            {
                if ((Settings.Contains("ChiefJudgeRole")))
                    return Settings["ChiefJudgeRole"].ToString();
                return "Chief Judge";
            }
        }
        public string CourtCounselRole
        {
            get
            {
                if ((Settings.Contains("CourtCounselRole")))
                    return Settings["CourtCounselRole"].ToString();
                return "Process Server Counsel";
            }
        }
        public string CardReplacementCost
        {
            get
            {
                if ((Settings.Contains("CardReplacementCost")))
                    return Settings["CardReplacementCost"].ToString();
                return "25";
            }
        }
        public string NewApplicationCost
        {
            get
            {
                if ((Settings.Contains("NewApplicationCost")))
                    return Settings["NewApplicationCost"].ToString();
                return "220";
            }
        }
        public string RenewedApplicationCost
        {
            get
            {
                if ((Settings.Contains("RenewedApplicationCost")))
                    return Settings["RenewedApplicationCost"].ToString();
                return "125";
            }
        }
        public string EnrollmentStart
        {
            get
            {
                if (EnrollmentStartDate != DateTime.MinValue)
                    return EnrollmentStartDate.ToString("MM/dd/yyyy");
                return "";
            }
        }
        public string EnrollmentEnd
        {
            get
            {
                if (EnrollmentEndDate != DateTime.MinValue)
                    return EnrollmentEndDate.ToString("MM/dd/yyyy");
                return "";
            }
        }
        /// <summary>
        /// Start of this year's enrollment window. Only the month and day of the stored setting are
        /// used - the year is replaced with the current year, so the window repeats annually and the
        /// setting does not have to be re-entered each year.
        /// </summary>
        public DateTime EnrollmentStartDate
        {
            get
            {
                DateTime start, end;
                GetEnrollmentWindow(out start, out end);
                return start;
            }
        }
        /// <summary>
        /// End of this year's enrollment window, on the same terms as <see cref="EnrollmentStartDate"/>.
        /// </summary>
        public DateTime EnrollmentEndDate
        {
            get
            {
                DateTime start, end;
                GetEnrollmentWindow(out start, out end);
                return end;
            }
        }
        /// <summary>
        /// The enrollment cycle applications are filed under. This rolls over when enrollment opens
        /// rather than on Jan 1, so an application still being processed after the new year keeps
        /// resolving to the cycle it was filed in.
        /// </summary>
        public int ApplicationYear
        {
            get
            {
                DateTime start = EnrollmentStartDate;
                if (start == DateTime.MinValue)
                    return DateTime.Today.Year;
                if (DateTime.Today < start)
                    return start.Year - 1;
                return start.Year;
            }
        }
        private void GetEnrollmentWindow(out DateTime start, out DateTime end)
        {
            start = Components.Helpers.OnCurrentYear(GetDateSetting("EnrollmentStart"));
            end = Components.Helpers.OnCurrentYear(GetDateSetting("EnrollmentEnd"));

            //a window configured across the new year (Nov 1 - Feb 28, say) inverts once both dates
            //sit on the same year, so move whichever end keeps today inside the window
            if (start != DateTime.MinValue && end != DateTime.MinValue && end < start)
            {
                if (DateTime.Today < start)
                    start = start.AddYears(-1);
                else
                    end = end.AddYears(1);
            }
        }
        private DateTime GetDateSetting(string key)
        {
            DateTime value = new DateTime();
            if (Settings.Contains(key))
                DateTime.TryParse(Settings[key].ToString(), out value);
            return value;
        }
        public string NotificationEmail
        {
            get
            {
                if ((Settings.Contains("NotificationEmail")))
                    return Settings["NotificationEmail"].ToString();
                return "process@jud12.flcourts.org";
            }
        }

        public bool PaymentReceived
        {
            get
            {
                var qs = Request.QueryString["paid"];
                if (qs != null)
                {
                    if (qs == "1")
                        return true;
                }
                return false;
            }
        }

        public const string ExternalUrlSetting = "ExternalUrl";

        /// <summary>
        /// The applicant-facing Process Server page on the public (jud12) site, from the ExternalUrl
        /// setting. Null when the setting is blank or is not an absolute http(s) address, so a bad value
        /// never becomes a link. Used for the "View your application" link in approve/reject emails.
        /// </summary>
        public string ExternalUrl
        {
            get { return ValidExternalUrl(Settings.Contains(ExternalUrlSetting) ? Settings[ExternalUrlSetting] as string : null); }
        }

        public static string ValidExternalUrl(string value)
        {
            Uri uri;
            value = (value ?? "").Trim();
            if (value.Length > 0 && Uri.TryCreate(value, UriKind.Absolute, out uri)
                && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
            {
                return uri.AbsoluteUri;
            }
            return null;
        }

        /// <summary>
        /// The applicant's own application page on the public site, for the approve/reject email. The entry
        /// control differs by application type - "new" (NewApplication.ascx) vs "renew" (RenewalApplication.ascx),
        /// matching how the public module links applicants (EditUrl("new") / EditUrl("renew")). ExternalUrl is the
        /// module's base page including "/mid/{moduleId}"; the control key is inserted before it as DNN renders it
        /// (e.g. .../Process-Server-Application/ctl/new/mid/1398). Null when ExternalUrl is not set.
        /// </summary>
        public string ApplicantLink(bool isRenewal)
        {
            string baseUrl = ExternalUrl;
            if (baseUrl == null) return null;

            string ctl = isRenewal ? "renew" : "new";
            int idx = baseUrl.IndexOf("/mid/", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
                return baseUrl.Substring(0, idx) + "/ctl/" + ctl + baseUrl.Substring(idx);
            return baseUrl.TrimEnd('/') + "/ctl/" + ctl;
        }
    }

}
