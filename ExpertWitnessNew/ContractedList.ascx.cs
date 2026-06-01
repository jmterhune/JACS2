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

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tjc.Modules.ExpertWitness.Components;

namespace tjc.Modules.ExpertWitness
{
    /// <summary>
    /// Public, read-only list of experts currently under contract, grouped by expert
    /// type (category) with a column per county showing coverage. Intended to be placed
    /// on a public-facing page so attorneys can see who is available.
    /// </summary>
    public partial class ContractedList : ExpertWitnessModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                    BindList();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindList()
        {
            var eCtl = new ExpertController();
            var lCtl = new LocationController();
            var tCtl = new TypeController();

            var counties = lCtl.GetLocations().OrderBy(l => l.LocationName).ToList();
            var categories = tCtl.GetTypes().OrderBy(t => t.TypeName).ToList();

            // "Under contract" = an explicit contract end date that is today or later.
            var contracted = eCtl.GetExperts()
                .Where(x => x.ContractEnds.HasValue && x.ContractEnds.Value.Date >= DateTime.Today)
                .ToList();

            // Last updated = the most recent add/edit among the contracted experts.
            DateTime? lastUpdated = contracted.Select(x => x.ModifiedDate ?? x.CreatedDate).Max();
            ltUpdated.Text = lastUpdated.HasValue
                ? string.Format("(Updated: {0:M/d/yyyy})", lastUpdated.Value)
                : string.Empty;

            // Resolve each contracted expert's categories and counties once.
            var expertTypes = contracted.ToDictionary(
                x => x.ExpertID,
                x => new HashSet<int>(eCtl.GetExpertTypeTypes(x.ExpertID).Select(t => t.TypeID)));
            var expertCounties = contracted.ToDictionary(
                x => x.ExpertID,
                x => new HashSet<int>(eCtl.GetExpertLocationLocations(x.ExpertID).Select(l => l.LocationID)));

            var sb = new StringBuilder();
            foreach (var category in categories)
            {
                var experts = contracted
                    .Where(x => expertTypes[x.ExpertID].Contains(category.TypeID))
                    .OrderBy(x => x.Description)
                    .ToList();
                if (experts.Count == 0)
                    continue;

                sb.Append("<h3 class=\"contracted-category\">").Append(Encode(category.TypeName)).Append("</h3>");
                sb.Append("<table class=\"table table-striped contracted-table\"><thead><tr><th>Expert</th>");
                foreach (var county in counties)
                    sb.Append("<th class=\"county-col\">Serves ").Append(Encode(county.LocationName)).Append(" County</th>");
                sb.Append("</tr></thead><tbody>");
                foreach (var expert in experts)
                {
                    sb.Append("<tr><td>").Append(Encode(expert.Description)).Append("</td>");
                    foreach (var county in counties)
                    {
                        bool serves = expertCounties[expert.ExpertID].Contains(county.LocationID);
                        sb.Append("<td class=\"county-col\">").Append(serves ? "X" : "").Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
            }

            ltList.Text = sb.Length > 0
                ? sb.ToString()
                : "<p>There are no experts currently under contract.</p>";
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
