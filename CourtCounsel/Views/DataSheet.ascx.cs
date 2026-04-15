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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class DataSheet : CourtCounselModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            liAdmin.Visible = IsAdmin;

            if (!IsPostBack)
            {
                BindAttorneyCheckBoxList();
                BindSheet();
            }
        }

        private void BindAttorneyCheckBoxList()
        {
            var ctrl = new AttorneyController();
            var attorneys = ctrl.GetActiveAttorneys().OrderBy(a => a.AttorneyName).ToList();

            cblAttorneys.Items.Clear();
            foreach (var att in attorneys)
            {
                cblAttorneys.Items.Add(new ListItem(att.AttorneyName, att.AttorneyName));
            }
        }

        private void BindSheet(List<string> selectedAttorneys = null)
        {
            var ctrl = new HistoryController();
            var data = ctrl.GetAllHistory();

            if (selectedAttorneys != null && selectedAttorneys.Any())
            {
                data = data.Where(h => selectedAttorneys.Contains(h.Responsible));
            }

            rptSheet.DataSource = data.OrderByDescending(h => h.DateReceived).ToList();
            rptSheet.DataBind();
        }

        protected void cmdFilter_Click(object sender, EventArgs e)
        {
            var selected = new List<string>();
            foreach (ListItem item in cblAttorneys.Items)
            {
                if (item.Selected)
                    selected.Add(item.Value);
            }

            BindSheet(selected);
        }

        protected void cmdClear_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in cblAttorneys.Items)
            {
                item.Selected = false;
            }

            BindSheet();
        }
    }
}
