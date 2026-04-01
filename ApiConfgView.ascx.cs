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
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI.WebControls;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from JACSModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class ApiConfig : JACSModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public ApiConfig()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                navbar.ModuleContext = this;
                navbar.ActiveLink = "lnkApiConfig";
                if (UserId <= 0 || !UserInfo.IsAdmin)
                {
                    Response.Redirect(_navigationManager.NavigateURL(), true);
                }
                if (!IsPostBack)
                {
                    BindActionTypeDropdown();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void BindActionTypeDropdown()
        {
            edit_type.Items.Clear();
            edit_type.Items.Add(new ListItem("Select Action", ""));

            foreach (ApiEndpointType type in Enum.GetValues(typeof(ApiEndpointType)))
            {
                var field = typeof(ApiEndpointType).GetField(type.ToString());
                var descriptionAttribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));
                string displayText = descriptionAttribute != null
                    ? descriptionAttribute.Description
                    : type.ToString();
                edit_type.Items.Add(new ListItem(displayText, ((int)type).ToString()));
            }
        }
    }
}