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
    /// The courtroom list the public cart request form picks from. The rows live in
    /// the public (jud12) database, reached through the "Jud12" connection.
    /// </summary>
    public class CourtroomController
    {
        private readonly IHostSettings _hostSettings;

        public CourtroomController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateCourtroom(Courtroom t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Courtroom>().Insert(t);
            }
        }

        public void UpdateCourtroom(Courtroom t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Courtroom>().Update(t);
            }
        }

        public void DeleteCourtroom(int courtroomId)
        {
            Courtroom t = GetCourtroom(courtroomId);
            if (t == null) return;

            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                ctx.GetRepository<Courtroom>().Delete(t);
            }
        }

        public Courtroom GetCourtroom(int courtroomId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                return ctx.GetRepository<Courtroom>().GetById(courtroomId);
            }
        }

        public IEnumerable<Courtroom> GetCourtrooms(int courthouseId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                return ctx.GetRepository<Courtroom>().Find("WHERE CourthouseId = @0", courthouseId).ToList();
            }
        }

        /// <summary>
        /// Every courtroom with the name of the courthouse it belongs to, from the
        /// tjc_presscart_courtrooms_by_courthouse view.
        /// </summary>
        public IEnumerable<CourtroomList> GetCourtrooms()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, PressCartController.CONN_JUD12))
            {
                return ctx.GetRepository<CourtroomList>().Get().ToList();
            }
        }
    }
}
