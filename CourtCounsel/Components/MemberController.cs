/*
' Copyright (c) 2022 Joe Terhune
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
using System.Linq;

namespace tjc.Modules.CourtCounsel.Components
{
    internal class MemberController
    {
        

        public void CreateMember(Member t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Member>();
                rep.Insert(t);
            }
        }

        public void DeleteMember(int memberId)
        {
            var t = GetMember(memberId);
            DeleteMember(t);
        }

        public void DeleteMember(Member t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Member>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Member> GetMembers()
        {
            IEnumerable<Member> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Member>();
                t = rep.Get();
            }
            return t;
        }
        /// <summary>
        /// judge=0,attorney=1
        /// </summary>
        /// <param name="typeId">Type of Member</param>
        /// <param name="active">Active or Not</param>
        /// <returns>List of Members</returns>
        public IEnumerable<Member> GetMembersByType(int typeId,bool active)
        {
            string whereClause = "Where MemberTypeId = @0";
            if (active)
            {
                whereClause+=" And Active = 1 ";
            }
            else
            {
                whereClause += " And Active = 0 ";

            }
            IEnumerable<Member> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Member>();
                t = rep.Find(whereClause,typeId).OrderByDescending(x => x.Active).ThenBy(x => x.LastName).ThenBy(x => x.FirstName);
            }
            return t;
        }
        public Member GetMember(int memberId)
        {
            Member t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Member>();
                t = rep.GetById(memberId);
            }
            return t;
        }
        public Member GetMemberByUserId(int userId)
        {
            Member t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Member>();
                t = rep.Find("Where UserId=@0",userId).FirstOrDefault();
            }
            return t;
        }
        public void UpdateMember(Member t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Member>();
                rep.Update(t);
            }
        }
       
    }
}
