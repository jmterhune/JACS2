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

namespace tjc.Modules.DeSoto.Probation.Components
{
    internal class EmailAddressController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateEmailAddress(EmailAddress t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<EmailAddress>();
                rep.Insert(t);
            }
        }

        public void DeleteEmailAddress(int emailAddressId)
        {
            var t = GetEmailAddress(emailAddressId);
            DeleteEmailAddress(t);
        }

        public void DeleteEmailAddress(EmailAddress t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<EmailAddress>();
                rep.Delete(t);
            }
        }

        public IEnumerable<EmailAddress> GetEmailAddresses()
        {
            IEnumerable<EmailAddress> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<EmailAddress>();
                t = rep.Get();
            }
            return t;
        }
        public bool EmailAddressExists(int emailAddressId)
        {
            EmailAddress t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<EmailAddress>();
                t = rep.GetById(emailAddressId);
            }
            return t.EmailAddressID > 0;
        }
        public EmailAddress GetEmailAddress(int emailAddressId)
        {
            EmailAddress t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<EmailAddress>();
                t = rep.GetById(emailAddressId);
            }
            return t;
        }
        public void UpdateEmailAddress(EmailAddress t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<EmailAddress>();
                rep.Update(t);
            }
        }
    }
}
