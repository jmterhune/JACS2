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

namespace tjc.Intranet.API.Components.CourtCounsel
{
    internal class LogEntryController 
    {
        private const string CONN_INTRANET = "Intranet.API"; //Connection

     
        public IEnumerable<LogEntry> GetLogEntrysByCaseNumber(string caseNumber)
        {
            IEnumerable<LogEntry> t;
            using (IDataContext ctx = DataContext.Instance(CONN_INTRANET))
            {
                var rep = ctx.GetRepository<LogEntry>();
                t = rep.Find("Where CaseNumber Like @0", caseNumber.Trim() + "%");
            }
            return t;
        }
    }
}
