/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using System;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class EditHistory : CourtCounselModuleBase
    {
        private int _logId;
        private string _caseNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            liAdmin.Visible = IsAdmin;

            _logId = LogId;
            _caseNumber = CaseNumber;

            if (!IsPostBack)
            {
                BindLists();

                if (_logId > 0)
                {
                    // Editing existing record
                    LoadRecord(_logId);
                    pnlFutureAction.Visible = true;
                    cmdDelete.Visible = true;
                }
                else if (!string.IsNullOrEmpty(_caseNumber))
                {
                    // New record for an existing case
                    txtCaseNumber.Text = _caseNumber;

                    // Try to pre-fill party name from existing records
                    var ctrl = new HistoryController();
                    var existing = ctrl.GetHistoryByCaseNumber(_caseNumber).FirstOrDefault();
                    if (existing != null)
                    {
                        txtCaseName.Text = existing.PartyName;
                    }
                }
            }
        }

        private void BindLists()
        {
            // Case Types
            var caseTypeCtrl = new CaseTypeController();
            drpCaseType.Items.Clear();
            drpCaseType.Items.Add(new ListItem("", ""));
            foreach (var ct in caseTypeCtrl.GetCaseTypes().OrderBy(c => c.CaseType))
            {
                drpCaseType.Items.Add(new ListItem(ct.CaseType, ct.CaseType));
            }

            // Counties
            var countyCtrl = new CountyController();
            drpCounty.Items.Clear();
            drpCounty.Items.Add(new ListItem("", ""));
            foreach (var c in countyCtrl.GetCounties().OrderBy(c => c.County))
            {
                drpCounty.Items.Add(new ListItem(c.County, c.County));
            }

            // Actions
            var actionCtrl = new ActionTakenController();
            drpAction.Items.Clear();
            drpAction.Items.Add(new ListItem("", ""));
            foreach (var a in actionCtrl.GetActions().OrderBy(a => a.Action))
            {
                drpAction.Items.Add(new ListItem(a.Action, a.Action));
            }

            // Requestors (active/inactive groups)
            BindActiveInactiveDropDown(drpRequestor, () =>
            {
                var ctrl = new RequestorController();
                var all = ctrl.GetRequestors().ToList();
                var active = all.Where(r => r.IsActive == true).OrderBy(r => r.RequestorName)
                    .Select(r => new ListItem(r.RequestorName, r.RequestorName)).ToList();
                var inactive = all.Where(r => r.IsActive != true).OrderBy(r => r.RequestorName)
                    .Select(r => new ListItem(r.RequestorName, r.RequestorName)).ToList();
                return (active, inactive);
            });

            // Attorneys (active/inactive groups)
            BindActiveInactiveDropDown(drpAttorney, () =>
            {
                var ctrl = new AttorneyController();
                var all = ctrl.GetAttorneys().ToList();
                var active = all.Where(a => a.IsActive == true).OrderBy(a => a.AttorneyName)
                    .Select(a => new ListItem(a.AttorneyName, a.AttorneyName)).ToList();
                var inactive = all.Where(a => a.IsActive != true).OrderBy(a => a.AttorneyName)
                    .Select(a => new ListItem(a.AttorneyName, a.AttorneyName)).ToList();
                return (active, inactive);
            });

            // Time Spent (active/inactive groups)
            BindActiveInactiveDropDown(drpTimeSpan, () =>
            {
                var ctrl = new TimeSpentController();
                var all = ctrl.GetTimeSpents().ToList();
                var active = all.Where(t => t.IsActive).OrderBy(t => t.TimeSpan)
                    .Select(t => new ListItem(t.TimeSpan, t.TimeSpan)).ToList();
                var inactive = all.Where(t => !t.IsActive).OrderBy(t => t.TimeSpan)
                    .Select(t => new ListItem(t.TimeSpan, t.TimeSpan)).ToList();
                return (active, inactive);
            });
        }

        private void BindActiveInactiveDropDown(DropDownList ddl,
            Func<(System.Collections.Generic.List<ListItem> active, System.Collections.Generic.List<ListItem> inactive)> getData)
        {
            var (active, inactive) = getData();

            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("", ""));

            // Active group marker
            ddl.Items.Add(new ListItem("--- Active ---", "<"));
            foreach (var item in active)
            {
                ddl.Items.Add(item);
            }

            // Inactive group marker
            if (inactive.Any())
            {
                ddl.Items.Add(new ListItem("--- Inactive ---", ">"));
                foreach (var item in inactive)
                {
                    ddl.Items.Add(item);
                }
            }
        }

        private void LoadRecord(int logId)
        {
            var ctrl = new HistoryController();
            var item = ctrl.GetHistory(logId);
            if (item == null)
            {
                Response.Redirect(SearchUrl);
                return;
            }

            txtDateReceived.Text = item.DateReceived.ToString("yyyy-MM-dd");
            txtCaseNumber.Text = item.CaseNumber;
            txtCaseName.Text = item.PartyName;

            SelectItemByText(drpCaseType, item.CaseType);
            SelectItemByText(drpRequestor, item.RequestedBy);
            SelectItemByText(drpAttorney, item.Responsible);

            if (item.MotionFiled.HasValue)
                txtMotionFiled.Text = item.MotionFiled.Value.ToString("yyyy-MM-dd");

            SelectItemByText(drpCounty, item.County);
            SelectItemByText(drpAction, item.Action);

            if (item.DateCompleted.HasValue)
                txtDateCompleted.Text = item.DateCompleted.Value.ToString("yyyy-MM-dd");

            SelectItemByText(drpTimeSpan, item.TimeSpent);
            SelectItemByText(drpStatus, item.StatusName);

            txtComments.Text = item.Comments;

            _caseNumber = item.CaseNumber;
        }

        private void SelectItemByText(DropDownList ddl, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var item = ddl.Items.FindByText(text);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var ctrl = new HistoryController();
            var item = new HistoryInfo
            {
                DateReceived = DateTime.Parse(txtDateReceived.Text),
                CaseNumber = txtCaseNumber.Text.Trim(),
                PartyName = txtCaseName.Text.Trim(),
                CaseType = drpCaseType.SelectedItem.Text,
                RequestedBy = drpRequestor.SelectedItem.Text,
                Responsible = drpAttorney.SelectedItem.Text,
                County = drpCounty.SelectedItem.Text,
                Action = drpAction.SelectedItem.Text,
                TimeSpent = drpTimeSpan.SelectedItem.Text,
                StatusName = drpStatus.SelectedItem.Text,
                Comments = txtComments.Text.Trim(),
                LastModifiedDate = DateTime.Now
            };

            if (!string.IsNullOrEmpty(txtMotionFiled.Text))
                item.MotionFiled = DateTime.Parse(txtMotionFiled.Text);

            if (!string.IsNullOrEmpty(txtDateCompleted.Text))
                item.DateCompleted = DateTime.Parse(txtDateCompleted.Text);

            if (_logId > 0)
            {
                item.LogId = _logId;
                ctrl.UpdateHistory(item);
            }
            else
            {
                ctrl.CreateHistory(item);
            }

            // Handle future action date - create a second record
            if (!string.IsNullOrEmpty(txtFutureAction.Text))
            {
                var futureItem = new HistoryInfo
                {
                    DateReceived = DateTime.Parse(txtFutureAction.Text),
                    CaseNumber = item.CaseNumber,
                    PartyName = item.PartyName,
                    CaseType = item.CaseType,
                    RequestedBy = item.RequestedBy,
                    Responsible = item.Responsible,
                    County = item.County,
                    StatusName = item.StatusName,
                    Comments = item.Comments,
                    LastModifiedDate = DateTime.Now
                };
                ctrl.CreateHistory(futureItem);
            }

            // Redirect to case history
            Response.Redirect(EditUrl("cn", item.CaseNumber, "CaseHistory"));
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_caseNumber))
            {
                Response.Redirect(EditUrl("cn", _caseNumber, "CaseHistory"));
            }
            else
            {
                Response.Redirect(SearchUrl);
            }
        }

        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            if (_logId > 0)
            {
                var ctrl = new HistoryController();
                var item = ctrl.GetHistory(_logId);
                string caseNum = item != null ? item.CaseNumber : string.Empty;

                ctrl.DeleteHistory(_logId);

                if (!string.IsNullOrEmpty(caseNum))
                {
                    Response.Redirect(EditUrl("cn", caseNum, "CaseHistory"));
                }
                else
                {
                    Response.Redirect(SearchUrl);
                }
            }
        }
    }
}
