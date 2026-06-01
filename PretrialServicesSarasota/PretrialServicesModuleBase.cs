/*
' Copyright (c) 2023  Joe Terhune
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.PretrialServices.Sarasota
{
    public class PretrialServicesModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public PretrialServicesModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        public int ItemId
        {
            get
            {
                var qs = Request.QueryString["did"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public string NavigateUrl { get { return _navigationManager.NavigateURL(); } }
        public DateTime? QueryDate
        {
            get
            {
                var qs = Request.QueryString["date"];
                if (qs != null)
                    return Convert.ToDateTime(qs);
                return null;
            }

        }
        public int LogId
        {
            get
            {
                var qs = Request.QueryString["lid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public DateTime IntakeDate
        {
            get
            {
                if (ViewState["IntakeDate"] != null)
                {
                    return DateTime.Parse(ViewState["IntakeDate"].ToString());
                }
                return DateTime.Now;
            }
            set
            {
                ViewState["IntakeDate"] = value;
            }

        }
    }
}