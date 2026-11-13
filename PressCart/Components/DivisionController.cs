/*
' Copyright (c) 2026  12th Judicial Circuit
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
using System.Linq;

namespace tjc.Modules.PressCart.Components
{
    /// <summary>
    /// The division / judge list the public cart request form picks from. The rows live
    /// in the public (jud12) database, reached through the "Jud12" connection.
    /// </summary>
    public class DivisionController
    {
        private readonly IHostSettings _hostSettings;

        public DivisionController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateDivision(Division t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Division>().Insert(t);
            }
        }

        public void UpdateDivision(Division t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Division>().Update(t);
            }
        }

        public void DeleteDivision(int divisionId)
        {
            Division t = GetDivision(divisionId);
            if (t == null) return;

            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Division>().Delete(t);
            }
        }

        public Division GetDivision(int divisionId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                return ctx.GetRepository<Division>().GetById(divisionId);
            }
        }

        public IEnumerable<Division> GetDivisions()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                return ctx.GetRepository<Division>().Get().OrderBy(d => d.DivisionName).ToList();
            }
        }
    }
}
