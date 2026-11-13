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
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.PressCart.Components
{
    /// <summary>
    /// Reads and writes the presentation cart requests, which live in the public
    /// (jud12) database rather than this intranet site's own.
    /// </summary>
    public class PressCartController
    {
        /// <summary>Connection string to the public site's database, from web.config.</summary>
        internal const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public PressCartController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        /// <summary>Every request, newest first.</summary>
        public IEnumerable<PressCart> GetRequests()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<PressCart>();
                return rep.Get().OrderByDescending(r => r.PressCartId).ToList();
            }
        }

        public PressCart GetRequest(int pressCartId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<PressCart>();
                return rep.GetById(pressCartId);
            }
        }

        public void UpdateRequest(PressCart t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<PressCart>();
                rep.Update(t);
            }
        }

        /// <summary>
        /// Records who moved the request to a new status and when. Returns false when the
        /// request has since been deleted by someone else.
        /// </summary>
        public bool SetStatus(int pressCartId, string status, string notes, int userId)
        {
            PressCart request = GetRequest(pressCartId);
            if (request == null) return false;

            request.Status = status;
            request.StatusNotes = notes;
            request.StatusChangedByUserId = userId;
            request.StatusChangedOnDate = DateTime.Now;
            UpdateRequest(request);
            return true;
        }

        public void DeleteRequest(int pressCartId)
        {
            PressCart t = GetRequest(pressCartId);
            if (t == null) return;

            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<PressCart>();
                rep.Delete(t);
            }
        }
    }
}
