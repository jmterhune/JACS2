/*
' Copyright (c) 2024 Joe Terhune
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
using tjc.Modules.ExpertWitness.Components;

namespace tjc.Modules.ExpertWitness.Components
{
    internal class TypeController
    {
        public void CreateType(Type t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Type>();
                rep.Insert(t);
            }
        }

        public void DeleteType(int typeId)
        {
            var t = GetType(typeId);
            DeleteType(t);
        }

        public void DeleteType(Type t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Type>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Type> GetTypes()
        {
            IEnumerable<Type> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Type>();
                t = rep.Get();
            }
            return t;
        }

        public Type GetType(int typeId)
        {
            Type t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Type>();
                t = rep.GetById(typeId);
            }
            return t;
        }

        public void UpdateType(Type t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Type>();
                rep.Update(t);
            }
        }
       
    }
}
