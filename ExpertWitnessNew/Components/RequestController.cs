/*
' Copyright (c) 2024 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.ExpertWitness.Components;

namespace tjc.Modules.ExpertWitness.Components
{
    internal class RequestController
    {
        public void CreateRequest(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Insert(t);
            }
        }

        public void DeleteRequest(int requestId)
        {
            var t = GetRequest(requestId);
            DeleteRequest(t);
        }

        public void DeleteRequest(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Request> GetRequests()
        {
            IEnumerable<Request> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<RequestListItem> GetRequestListItems()
        {
            IEnumerable<RequestListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestListItem>();
                t = rep.Get().OrderByDescending(x=> x.RequestID);
            }
            return t;
        }
        public RequestListItem GetRequestListItem(int requestId)
        {
            RequestListItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RequestListItem>();
                t = rep.GetById(requestId);
            }
            return t;
        }
        public Request GetRequest(int requestId)
        {
            Request t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                t = rep.GetById(requestId);
            }
            return t;
        }

        public void UpdateRequest(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Update(t);
            }
        }
    }
}
