/*
' Copyright (c) 2023 jterhune
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

namespace tjc.Modules.JacsCaseMaint.Components
{
    internal class InterfaceMessageController
    {

        private const string CONN_INTRANET = "jacsManatee";

        public IEnumerable<InterfaceMessage> GetMessages(string year,string caseType, string sequence)
        {
            IEnumerable<InterfaceMessage> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                string casenumber = string.Format("%{0}%{1}%{2}%", year, caseType, sequence);
                var rep = ctx.GetRepository<InterfaceMessage>();
                t = rep.Find("Where CaseNumber like @0", casenumber);
            }
            return t;
        }

        public InterfaceMessage GetMessage(int messagId)
        {
            InterfaceMessage t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<InterfaceMessage>();
                t = rep.GetById(messagId);
            }
            return t;
        }
    }
}
