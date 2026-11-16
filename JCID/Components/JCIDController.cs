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
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;

namespace tjc.Modules.JCID.Components
{
    class JCIDController
    {
        // The JCID tables live in the jud12 (external/public) site's database. This intranet admin module
        // reads and writes them through the Jud12 named connection string, configured in the site web.config.
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public JCIDController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<JCIDInfo> GetItems()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<JCIDInfo>().Get();
            }
        }

        public JCIDInfo GetItem(int itemId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<JCIDInfo>().GetById(itemId);
            }
        }

        public void UpdateItem(JCIDInfo t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<JCIDInfo>().Update(t);
            }
        }
    }
}
