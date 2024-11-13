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
    internal class ProbationClassController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateProbationClass(ProbationClass t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationClass>();
                rep.Insert(t);
            }
        }

        public void DeleteProbationClass(int classId)
        {
            var t = GetProbationClass(classId);
            DeleteProbationClass(t);
        }

        public void DeleteProbationClass(ProbationClass t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationClass>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ProbationClass> GetProbationClasses()
        {
            IEnumerable<ProbationClass> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationClass>();
                t = rep.Get();
            }
            return t;
        }
        public bool ProbationClassExists(int classId)
        {
            ProbationClass t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationClass>();
                t = rep.GetById(classId);
            }
            return t.ClassID > 0;
        }
        public ProbationClass GetProbationClass(int classId)
        {
            ProbationClass t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationClass>();
                t = rep.GetById(classId);
            }
            return t;
        }
        public void UpdateProbationClass(ProbationClass t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<ProbationClass>();
                rep.Update(t);
            }
        }
    }
}
