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
    public partial class CourtroomView : JACSModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public CourtroomView()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                navbar.MainViewUrl = MainViewUrl;
                navbar.AttorneyListUrl = AttorneyListUrl;
                navbar.CourtroomListUrl = CourtroomListUrl;
                navbar.CountyListUrl = CountyListUrl;
                navbar.CourtListUrl = CourtListUrl;
                navbar.CourtTypeListUrl = CourtTypeListUrl;
                navbar.CourtPermissionListUrl = CourtPermissionListUrl;
                navbar.DocketPrintUrl = DocketPrintUrl;
                navbar.EventListUrl = EventListUrl;
                navbar.EventStatusListUrl = EventStatusListUrl;
                navbar.EventTypeListUrl = EventTypeListUrl;
                navbar.HolidayListUrl = HolidayListUrl;
                navbar.JudgeListUrl = JudgeListUrl;
                navbar.MotionListUrl = MotionListUrl;
                navbar.TemplateListUrl = TemplateListUrl;
                navbar.TimeSlotListUrl = TimeSlotListUrl;
                navbar.QuickReferenceUrl = QuickReferenceUrl;
                navbar.UserListUrl = UserListUrl;
                navbar.RoleListUrl = RoleListUrl;
                navbar.PermissionListUrl = PermissionListUrl;
                navbar.ActiveLink = "lnkCourtroom";
                if (UserId <= 0 || !UserInfo.IsAdmin)
                {
                    Response.Redirect(_navigationManager.NavigateURL(), true);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}