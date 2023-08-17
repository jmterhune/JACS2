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

namespace tjc.Modules.FamilySelfHelp.Components
{
    internal class ClientController
    {
        public void CreateClient(Client t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Client>();
                rep.Insert(t);
            }
        }

        public void DeleteClient(long clientId)
        {
            var t = GetClient(clientId);
            DeleteClient(t);
        }

        public void DeleteClient(Client t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Client>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Client> GetClients()
        {
            IEnumerable<Client> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Client>();
                t = rep.Get();
            }
            return t;
        }

        public IEnumerable<ClientName> GetClientNames(string name)
        {
            IEnumerable<ClientName> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = "SELECT  [LastName] + ', ' + [FirstName] As 'Text', ClientId as 'Value' FROM tjc_shc_Client WHERE ([LastName] + ', ' + [FirstName] ) LIKE '%@0%' ORDER BY [LastName], [FirstName]";
               t= ctx.ExecuteQuery<ClientName>(System.Data.CommandType.Text,sql,name);
            }
            return t;
        }

        public Client GetClient(long clientId)
        {
            Client t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Client>();
                t = rep.Find("Where ClientId=@0",clientId).FirstOrDefault();
            }
            return t;
        }

        public void UpdateClient(Client t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Client>();
                rep.Update(t);
            }
        }

    }
}
