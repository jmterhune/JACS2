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
    internal class ProgramController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateProgram(Program t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Program>();
                rep.Insert(t);
            }
        }

        public void DeleteProgram(int programId)
        {
            var t = GetProgram(programId);
            DeleteProgram(t);
        }

        public void DeleteProgram(Program t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Program>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Program> GetPrograms()
        {
            IEnumerable<Program> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Program>();
                t = rep.Get();
            }
            return t;
        }
        public bool ProgramExists(int programId)
        {
            Program t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Program>();
                t = rep.GetById(programId);
            }
            return t.ProgramID > 0;
        }
        public Program GetProgram(int programId)
        {
            Program t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Program>();
                t = rep.GetById(programId);
            }
            return t;
        }
        public void UpdateProgram(Program t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Program>();
                rep.Update(t);
            }
        }
    }
}
