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
    internal class PaymentTypeController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreatePaymentType(PaymentType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<PaymentType>();
                rep.Insert(t);
            }
        }

        public void DeletePaymentType(int paymentTypeId)
        {
            var t = GetPaymentType(paymentTypeId);
            DeletePaymentType(t);
        }

        public void DeletePaymentType(PaymentType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<PaymentType>();
                rep.Delete(t);
            }
        }

        public IEnumerable<PaymentType> GetPaymentTypees()
        {
            IEnumerable<PaymentType> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<PaymentType>();
                t = rep.Get();
            }
            return t;
        }
        public bool PaymentTypeExists(int paymentTypeId)
        {
            PaymentType t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<PaymentType>();
                t = rep.GetById(paymentTypeId);
            }
            return t.PaymentTypeID > 0;
        }
        public PaymentType GetPaymentType(int paymentTypeId)
        {
            PaymentType t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<PaymentType>();
                t = rep.GetById(paymentTypeId);
            }
            return t;
        }
        public void UpdatePaymentType(PaymentType t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<PaymentType>();
                rep.Update(t);
            }
        }
    }
}
