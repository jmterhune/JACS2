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

using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using tjc.Modules.CourtCounsel.Components;

namespace tjc.Modules.CourtCounsel
{
    public partial class EditEvent : CourtCounselModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public EditEvent()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    if (EventId > 0)
                    {
                        var ctl = new EventController();
                        Event @event = ctl.GetEvent(EventId);
                        if (@event != null)
                        {
                            txtStartDate.Text = @event.StartDate.ToString();
                            txtSubject.Text = @event.Subject.ToString();
                            txtBody.Text = @event.Body.ToString();
                            txtReminderDays.Text= @event.ReminderDays.ToString();
                            hdExternalId.Value = @event.ExternalId;
                        }
                        else
                        {
                            txtReminderDays.Text = DefaultReminderPeriod.ToString();
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var ctl = new EventController();
            DateTime.TryParse(txtStartDate.Text, out DateTime startDate);
            Int32.TryParse(txtReminderDays.Text, out int reminderDays);
            Event @event = new Event { ExternalId = hdExternalId.Value, AssignmentId = AssignmentId, Subject = txtSubject.Text, Body = txtBody.Text, StartDate = startDate, EndDate = startDate.AddDays(1), IsAllDay = true, IsReminderOn = true, ReminderMinutesBeforeStart = reminderDays * 1440, UserName = UserInfo.Email, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, CreatedBy = UserInfo.Username, ModifiedBy = UserInfo.Username, };
            if (!string.IsNullOrEmpty(hdExternalId.Value))
            {
                ctl.UpdateEvent(@event, UserInfo.Email, PortalId);
            }
            else
            {
                ctl.CreateEvent(@event, UserInfo.Email, PortalId);

            }
            Response.Redirect(_navigationManager.NavigateURL());
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(_navigationManager.NavigateURL());
        }
    }
}