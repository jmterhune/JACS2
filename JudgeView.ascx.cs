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

using DotNetNuke.Services.Exceptions;
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
    public partial class JudgeView : JACSModuleBase
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
                navbar.ActiveLink = "lnkJudge";
                //if (!IsPostBack)
                //{
                //    int portalId = PortalSettings.PortalId;
                //    var roleController = new DotNetNuke.Security.Roles.RoleController();
                //    RoleInfo judgeRole = roleController.GetRoleByName(portalId, JudgeRole);
                //    if (judgeRole != null)
                //    {
                //        // Get users in the "Judge" role
                //        var usersInRole = roleController.GetUsersByRole(portalId, JudgeRole);

                //        // Clear existing items in the dropdown
                //        edit_judgeName.Items.Clear();

                //        // Add a default "Select User" option
                //        edit_judgeName.Items.Add(new ListItem("Select Judge", ""));

                //        // Populate the dropdown with users
                //        foreach (UserInfo user in usersInRole)
                //        {
                //            edit_judgeName.Items.Add(new ListItem(user.DisplayName, user.UserID.ToString()));
                //        }
                //    }
                //    else
                //    {
                //        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this,"The Judge Role has not been defined", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                //    }
                //}
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}