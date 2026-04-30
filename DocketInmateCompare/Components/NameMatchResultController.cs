// File: Components\NameMatchResultController.cs
/*
' Copyright (c) 2026 Joe Terhune
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace tjc.Modules.DocketInmateCompare.Components
{
    internal class NameMatchResultController
    {
        public void CreateItem(NameMatchResult t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<NameMatchResult>();
                rep.Insert(t);
            }
        }

        public void DeleteItem(int itemId)
        {
            var t = GetItem(itemId);
            DeleteItem(t);
        }

        public void DeleteItem(NameMatchResult t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<NameMatchResult>();
                rep.Delete(t);
            }
        }

        public IEnumerable<NameMatchResult> GetItems()
        {
            IEnumerable<NameMatchResult> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<NameMatchResult>();
                t = rep.Get();
            }
            return t;
        }

        public NameMatchResult GetItem(int itemId)
        {
            NameMatchResult t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<NameMatchResult>();
                t = rep.GetById(itemId);
            }
            return t;
        }

        public void UpdateItem(NameMatchResult t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<NameMatchResult>();
                rep.Update(t);
            }
        }

        public IEnumerable<NameMatchResult> GetItemsBySetGuid( Guid setGuid)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<NameMatchResult>();
                var items = rep.Find("WHERE SetGuid = @0",  setGuid);
                return items.OrderByDescending(i => i.Similarity).ThenBy(i => i.CourtName.ToUpper());
            }
        }

        public void DeleteItemsBySetGuid( Guid setGuid)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text, "DELETE FROM {databaseOwner}{objectQualifier}tjc_inmate_matches WHERE SetGuid = @0",  setGuid);
            }
        }
    }
}