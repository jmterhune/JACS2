/*
' Copyright (c) 2022  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Modules;
using System;

namespace tjc.Modules.JudicialReferral
{
    public class JudicialReferralModuleBase : PortalModuleBase
    {
        public string JudgeRole
        {
            get
            {
                if ((Settings.Contains("JudgeRole")))
                    return Settings["JudgeRole"].ToString();
                return "Judge";
            }
        }
        public int ReferralID
        {
            get
            {
                var qs = Request.QueryString["rid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }

        public string JaRole
        {
            get
            {
                if (Settings.Contains("JaRole"))
                    return Settings["JaRole"].ToString();
                return "Ja";
            }
        }
        public string CounselRole
        {
            get
            {
                if (Settings.Contains("CounselRole"))
                    return Settings["CounselRole"].ToString();
                return "Court Counsel";
            }
        }

        public string TargetFolder
        {
            get
            {
                if (Settings.Contains("FolderName"))
                    return Settings["FolderName"].ToString();
                return "Judicial-Referral-Attachments";
            }
        }
        public string CourtCounselEmail
        {
            get
            {
                if (Settings.Contains("CourtCounselEmail"))
                    return Settings["CourtCounselEmail"].ToString();
                return "jterhune@jud12.flcourts.org";
            }
        }
        public bool IsJudge
        {
            get
            {
                return UserInfo.IsInRole(JudgeRole);
            }
        }
        public bool IsJa
        {
            get
            {
                return UserInfo.IsInRole(JaRole);
            }
        }
        public bool IsCounsel
        {
            get
            {
                return UserInfo.IsInRole(CounselRole);
            }
        }
        public long MaxRequestLength {
            get { return DotNetNuke.Common.Utilities.Config.GetMaxUploadSize(); } }
        public string MaxFileSize
        {
            get { return string.Format("{0}MB",DotNetNuke.Common.Utilities.Config.GetMaxUploadSize() / 1000000); }
        }
    }
}