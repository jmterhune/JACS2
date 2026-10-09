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
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;


namespace tjc.Modules.ThreatReport.Components
{
    class PersonController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        private readonly IHostSettings _hostSettings;

        public PersonController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreatePerson(Person t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Person>();
                rep.Insert(t);
            }
        }

        public void DeletePerson(int personId)
        {
            var t = GetPerson(personId);
            DeletePerson(t);
        }

        public void DeletePerson(Person t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Person>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Person> GetPersons(int id)
        {
            IEnumerable<Person> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Person>();
                t = rep.Find("Where IncidentID=@0", id);
            }
            return t;
        }

        public Person GetPerson(int personId)
        {
            Person t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Person>();
                t = rep.GetById(personId);
            }
            return t;
        }

        public void UpdatePerson(Person t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Person>();
                rep.Update(t);
            }
        }

    }
}
