/*
' Copyright (c) 2019 jud12
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

namespace tjc.Modules.ThreatReport.Components
{
    class IncidentController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection

        public void CreateIncident(Incident t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Incident>();
                rep.Insert(t);
            }
        }

        public void DeleteIncident(int incidentId)
        {
            var t = GetIncident(incidentId);
            DeleteIncident(t);
        }

        public void DeleteIncident(Incident t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Incident>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Incident> GetIncidents()
        {
            IEnumerable<Incident> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Incident>();
                t = rep.Get().OrderByDescending(i => i.IncidentID);
            }
            return t;
        }

        public IEnumerable<Attachment> GetIncidentAttachments(int incidentID)
        {
            IEnumerable<Attachment> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Find("Where IncidentID = @0", incidentID);
            }
            return t;
        }
        public IEnumerable<Person> GetInvolvedPersons(int incidentID)
        {
            IEnumerable<Person> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Person>();
                t = rep.Find("Where IncidentID = @0", incidentID);
            }
            return t;
        }
        public Incident GetIncident(int incidentId)
        {
            Incident t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Incident>();
                t = rep.GetById(incidentId);
            }
            return t;
        }

        public void UpdateIncident(Incident t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Incident>();
                rep.Update(t);
            }
        }

    }
}
