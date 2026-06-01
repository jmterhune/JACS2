/*
' Copyright (c) 2024  Joe Terhune
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

namespace tjc.Modules.ExpertWitness
{
    public class ExpertWitnessModuleBase : PortalModuleBase
    {
        public int RequestId
        {
            get
            {
                var qs = Request.QueryString["rid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public string ReportUrl
        {
            get
            {
                if (Settings.Contains("ReportUrl"))
                    return Settings["ReportUrl"].ToString();
                return "";
            }
        }
        public string RequestListUrl { get { return EditUrl("request").ToString(); } }
        public string ExpertListUrl { get { return EditUrl("expert").ToString(); } }
        public string EvaluationTypeListUrl { get { return EditUrl("evaluation").ToString(); } }
        public string TypeListUrl { get { return EditUrl("type").ToString(); } }
        public string LocationListUrl { get { return EditUrl("location").ToString(); } }
        public string AdminRole
        {
            get
            {
                if (Settings.Contains("AdminRole"))
                    return Settings["AdminRole"].ToString();
                return "";
            }
        }
        public bool IsAdmin
        {
            get
            {
                if (UserId > 0)
                    return UserInfo.IsInRole(AdminRole);
                return false;
            }
        }

    }
}