/*
' Copyright (c) 2026 jterhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace tjc.Modules.JacsCaseMaint.Components
{
    internal class AttorneyController
    {
        private const string CONN_INTRANET = "jacsDesoto";
        private const int BAR_NUMBER_LENGTH = 7;
        private readonly IHostSettings _hostSettings;

        public AttorneyController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        /// <summary>
        /// JACS bar numbers are 7 characters, left padded with zeros.
        /// </summary>
        public static string PadBarNumber(string barNumber)
        {
            return (barNumber ?? string.Empty).Trim().PadLeft(BAR_NUMBER_LENGTH, '0');
        }

        public IEnumerable<Attorney> GetAttorneyByBarNumber(string barNumber)
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_INTRANET))
            {
                t = ctx.ExecuteQuery<Attorney>(CommandType.StoredProcedure, "jacs.tjc_get_attorney_by_barnumber", PadBarNumber(barNumber)).ToList();
            }
            return t;
        }

        /// <summary>
        /// Sets ACTIVE to 'Y' or 'N'. The stored procedure updates the attorney in all three JACS databases.
        /// </summary>
        public void SetActive(string barNumber, bool active)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_INTRANET))
            {
                ctx.Execute(CommandType.StoredProcedure, "jacs.tjc_update_attorney_active_status", PadBarNumber(barNumber), active ? "Y" : "N");
            }
        }
    }
}
