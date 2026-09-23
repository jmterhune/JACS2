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
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.DeSoto.Probation.Components
{
    internal class PhoneController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        private readonly IHostSettings _hostSettings;

        public PhoneController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreatePhone(Phone t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Phone>();
                rep.Insert(t);
            }
        }

        public void DeletePhone(int phoneId)
        {
            var t = GetPhone(phoneId);
            DeletePhone(t);
        }

        public void DeletePhone(Phone t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Phone>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Phone> GetPhonees()
        {
            IEnumerable<Phone> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.Get();
            }
            return t;
        }
        public bool PhoneExists(int phoneId)
        {
            Phone t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.GetById(phoneId);
            }
            return t.PhoneID > 0;
        }
        public Phone GetPhone(int phoneId)
        {
            Phone t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Phone>();
                t = rep.GetById(phoneId);
            }
            return t;
        }
        public void UpdatePhone(Phone t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Phone>();
                rep.Update(t);
            }
        }
    }
}
