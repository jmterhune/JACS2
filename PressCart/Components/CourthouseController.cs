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
    /// The courthouse list the public cart request form picks from. The rows live in
    /// the public (jud12) database, reached through the "Jud12" connection.
    /// </summary>
    public class CourthouseController
    {
        private readonly IHostSettings _hostSettings;

        public CourthouseController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateCourthouse(Courthouse t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Courthouse>().Insert(t);
            }
        }

        public void UpdateCourthouse(Courthouse t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Courthouse>().Update(t);
            }
        }

        public void DeleteCourthouse(int courthouseId)
        {
            Courthouse t = GetCourthouse(courthouseId);
            if (t == null) return;

            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Courthouse>().Delete(t);
            }
        }

        public IEnumerable<Courthouse> GetCourthouses()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                return ctx.GetRepository<Courthouse>().Get().OrderBy(c => c.CourthouseName).ToList();
            }
        }

        public Courthouse GetCourthouse(int courthouseId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                return ctx.GetRepository<Courthouse>().GetById(courthouseId);
            }
        }

        /// <summary>
        /// True when a courtroom still points at this courthouse. Deleting it anyway
        /// would leave those courtrooms unreachable from the public form.
        /// </summary>
        public bool HasCourtrooms(int courthouseId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                return ctx.GetRepository<Courtroom>().Find("WHERE CourthouseId = @0", courthouseId).Any();
            }
        }
    }
}
