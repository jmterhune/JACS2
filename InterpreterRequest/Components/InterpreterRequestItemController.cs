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

namespace tjc.Modules.InterpreterRequest.Components
{
    /// <summary>
    /// Reads and writes the interpreter requests, which live in the public (jud12)
    /// database rather than this intranet site's own.
    /// </summary>
    public class InterpreterRequestItemController
    {
        /// <summary>Connection string to the public site's database, from web.config.</summary>
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public InterpreterRequestItemController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        /// <summary>Every request, newest first.</summary>
        public IEnumerable<InterpreterRequestItem> GetRequests()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<InterpreterRequestItem>();
                return rep.Get().OrderByDescending(r => r.InterpreterRequestId).ToList();
            }
        }

        public InterpreterRequestItem GetRequest(int interpreterRequestId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<InterpreterRequestItem>();
                return rep.GetById(interpreterRequestId);
            }
        }

        public void UpdateRequest(InterpreterRequestItem t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<InterpreterRequestItem>();
                rep.Update(t);
            }
        }

        /// <summary>
        /// Records who moved the request to a new status and when. Returns false when the
        /// request has since been deleted by someone else.
        /// </summary>
        public bool SetStatus(int interpreterRequestId, string status, string notes, int userId)
        {
            InterpreterRequestItem request = GetRequest(interpreterRequestId);
            if (request == null) return false;

            request.Status = status;
            request.StatusNotes = notes;
            request.StatusChangedByUserId = userId;
            request.StatusChangedOnDate = DateTime.Now;
            UpdateRequest(request);
            return true;
        }

        public void DeleteRequest(int interpreterRequestId)
        {
            InterpreterRequestItem t = GetRequest(interpreterRequestId);
            if (t == null) return;

            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<InterpreterRequestItem>();
                rep.Delete(t);
            }
        }
    }
}
