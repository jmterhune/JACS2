/*
' Copyright (c) 2023 Joe Terhune
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

namespace tjc.Modules.EquipmentRequest.Components
{
    internal class RequestController
    {
        public void CreateItem(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Insert(t);
            }
        }

        public void DeleteItem(int itemId, int moduleId)
        {
            var t = GetItem(itemId, moduleId);
            DeleteItem(t);
        }

        public void DeleteItem(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Request> GetItems(int moduleId)
        {
            IEnumerable<Request> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                t = rep.Get(moduleId);
            }
            return t;
        }

        public Request GetItem(int itemId, int moduleId)
        {
            Request t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                t = rep.GetById(itemId, moduleId);
            }
            return t;
        }

        public void UpdateItem(Request t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Request>();
                rep.Update(t);
            }
        }

    }
}
