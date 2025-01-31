/*
' Copyright (c) 2025  Joe Terhune
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

namespace tjc.Modules.RecordDestruction
{
    public class RecordDestructionModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public RecordDestructionModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }
        public string AttachmentDirectory
        {
            get
            {
                if (Settings.Contains("AttachmentFolderName"))
                    return Settings["AttachmentFolderName"].ToString();
                return "DestructionLog";
            }
        }
        public bool IsAdmin
        {
            get
            {
                if (UserInfo.IsInRole(AdminRole))
                    return true;
                return false;
            }
        }
        public string AdminRole
        {
            get
            {
                if (Settings.Contains("AdminRole"))
                    return Settings["AdminRole"].ToString();
                return "Court Administration";
            }
        }
        public string DestructionFormURL { get { return _navigationManager.NavigateURL(); } }
        public string SearchLogUrl { get { return EditUrl("search"); } }
        public string DepartmentListUrl { get { return EditUrl("group-list"); } }

        public string RetentionPeriodListUrl { get { return EditUrl("retention-list"); } }

        public string DestructionMethodListUrl { get { return EditUrl("method-list"); } }

        public string RecordTypeListUrl { get { return EditUrl("type-list"); } }

    }
}