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

namespace tjc.Intranet.API.Components.FamilySelfHelp
{
    internal class ClientController
    {
        public IEnumerable<ClientName> GetClientNames(string name)
        {
            IEnumerable<ClientName> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT  [LastName] + ', ' + [FirstName] As 'Text', ClientId as 'Value' FROM tjc_shc_Client WHERE ([LastName] + ', ' + [FirstName] ) LIKE @0 ORDER BY [LastName], [FirstName]";
               t= ctx.ExecuteQuery<ClientName>(System.Data.CommandType.Text,sql,string.Format("%{0}%",name));
            }
            return t;
        }
    }
}
