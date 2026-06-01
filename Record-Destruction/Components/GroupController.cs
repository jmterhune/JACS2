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
using System.Linq;

namespace tjc.Modules.RecordDestruction.Components
{
    internal class GroupController
    {
        public void CreateGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Insert(t);
            }
        }

        public void DeleteGroup(int groupId)
        {
            var t = GetGroup(groupId);
            DeleteGroup(t);
        }

        public void DeleteGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Delete(t);
            }
        }             
        public IEnumerable<Group> GetGroups()
        {
            IEnumerable<Group> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                t = rep.Find("Where GroupType=0").OrderBy(x=>x.GroupName);
            }
            return t;
        }
        public Group GetGroup(int groupId)
        {
            Group t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                t = rep.GetById(groupId);
            }
            return t;
        }

        public void UpdateGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Update(t);
            }
        }
    }
}
