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
    internal class AddressController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateAddress(Address t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Address>();
                rep.Insert(t);
            }
        }

        public void DeleteAddress(int addressId)
        {
            var t = GetAddress(addressId);
            DeleteAddress(t);
        }

        public void DeleteAddress(Address t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Address>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Address> GetAddresses()
        {
            IEnumerable<Address> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Address>();
                t = rep.Get();
            }
            return t;
        }
        public bool AddressExists(int addressId)
        {
            Address t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Address>();
                t = rep.GetById(addressId);
            }
            return t.AddressID > 0;
        }
        public Address GetAddress(int addressId)
        {
            Address t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Address>();
                t = rep.GetById(addressId);
            }
            return t;
        }
        public void UpdateAddress(Address t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Address>();
                rep.Update(t);
            }
        }
    }
}
