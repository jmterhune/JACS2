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

namespace tjc.Modules.RecordDestruction.Components
{
    internal class DestructionMethodController
    {
        public void CreateDestructionMethod(DestructionMethod t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DestructionMethod>();
                rep.Insert(t);
            }
        }

        public void DeleteDestructionMethod(int destructionMethodId)
        {
            var t = GetDestructionMethod(destructionMethodId);
            DeleteDestructionMethod(t);
        }

        public void DeleteDestructionMethod(DestructionMethod t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DestructionMethod>();
                rep.Delete(t);
            }
        }             
        public IEnumerable<DestructionMethod> GetDestructionMethods()
        {
            IEnumerable<DestructionMethod> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DestructionMethod>();
                t = rep.Get();
            }
            return t;
        }
        public DestructionMethod GetDestructionMethod(int destructionMethodId)
        {
            DestructionMethod t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DestructionMethod>();
                t = rep.GetById(destructionMethodId);
            }
            return t;
        }

        public void UpdateDestructionMethod(DestructionMethod t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DestructionMethod>();
                rep.Update(t);
            }
        }
    }
}
