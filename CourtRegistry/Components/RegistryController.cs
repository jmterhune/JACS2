/*
' Copyright (c) 2025 Joe Terhune
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

namespace tjc.Modules.CourtRegistry.Components
{
    internal class RegistryController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateRegistry(Registry t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Registry>();
                rep.Insert(t);
            }
        }

        public void DeleteRegistry(int registryId)
        {
            var t = GetRegistry(registryId);
            DeleteRegistry(t);
        }

        public void DeleteRegistry(Registry t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Registry>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Registry> GetRegistrys()
        {
            IEnumerable<Registry> t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Registry>();
                t = rep.Get();
            }
            return t;
        }

        public Registry GetRegistry(int registryId)
        {
            Registry t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Registry>();
                t = rep.GetById(registryId);
            }
            return t;
        }

        public void UpdateRegistry(Registry t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Registry>();
                rep.Update(t);
            }
        }

    }
}
