/*
' Copyright (c) 2017  12th Judicial Circuit
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/


using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using System;
using tjc.Modules.JCID.Components;

namespace tjc.Modules.JCID
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The default admin view: the list of JCID applications and the credit-card payments. Reads the jud12
    /// (public) site's database through the Jud12 connection. Each application links to the Edit control.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class TransactionList : JCIDModuleBase
    {
        private DateTime startDate = DateTime.Now.AddDays(-30);
        private DateTime endDate = DateTime.Now;
        #region Event Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    txtStartDate.Text = startDate.ToShortDateString();
                    txtEndDate.Text = endDate.ToShortDateString();
                    if (!DotNetNuke.Security.Permissions.ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", ModuleConfiguration))
                    {
                        Response.Redirect("/");
                    }

                    var ctl = new TransactionController(_hostSettings);
                    rptTransactions.DataSource = ctl.GetTransactionsByDateRange(startDate, endDate.AddDays(1));
                    rptTransactions.DataBind();
                    var aCtl = new JCIDController(_hostSettings);
                    rptApps.DataSource = aCtl.GetItems();
                    rptApps.DataBind();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }


        #endregion

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            DateTime.TryParse(txtStartDate.Text, out DateTime start);
            DateTime.TryParse(txtEndDate.Text, out DateTime end);
            startDate = start;
            endDate = end;
            var ctl = new TransactionController(_hostSettings);
            rptTransactions.DataSource = ctl.GetTransactionsByDateRange(startDate, endDate.AddDays(1));
            rptTransactions.DataBind();

        }

        /// <summary>
        /// The public payment page link for an application, so staff can copy it for an applicant who still
        /// owes payment. Null (and the copy icon is hidden) when the External URL setting is not configured.
        /// </summary>
        protected string PaymentLink(object dataItem)
        {
            return ExternalPaymentUrl(((JCIDInfo)dataItem).RequestID);
        }
    }
}
