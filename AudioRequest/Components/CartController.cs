/*
' Copyright (c) 2017 12th Judicial Circuit
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using System.Collections.Generic;
using DotNetNuke.Data;

namespace tjc.Modules.AudioRequest.Components
{
    class CartController
    {
        private const string CONN_INTRANET = "Intranet";
        public void CreateItem(CartInfo t)
        {
            using (IDataContext ctx = DataContext.Instance()) //add CONN_INTRANET in instance bracket Instance(CONN_INTRANET)
            {
                var rep = ctx.GetRepository<CartInfo>();
                rep.Insert(t);
            }
        }

    }
}
