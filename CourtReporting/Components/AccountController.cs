/*
' Copyright (c) 2025 Joe Terhune
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

namespace tjc.Modules.CourtReporting.Components
{
    internal class AccountController
    {
        public void CreateAccount(Account t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                rep.Insert(t);
            }
        }

        public void DeleteAccount(int accountId)
        {
            var t = GetAccount(accountId);
            DeleteAccount(t);
        }

        public void DeleteAccount(Account t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Account> GetAccounts()
        {
            IEnumerable<Account> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                t = rep.Get();
            }
            return t;
        }

        public Account GetAccount(int accountId)
        {
            Account t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                t = rep.GetById(accountId);
            }
            return t;
        }

        public void UpdateAccount(Account t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Account>();
                rep.Update(t);
            }
        }

    }
}
