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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using System;
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
    public partial class EventStatusView : JACSModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                navbar.MainViewUrl = MainViewUrl;
                navbar.AttorneyListUrl = AttorneyListUrl;
                navbar.CategoryListUrl = CategoryListUrl;
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
                navbar.ActiveLink = "lnkEventStatus";
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}