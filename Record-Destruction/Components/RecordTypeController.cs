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
    internal class RecordTypeController
    {
        public void CreateRecordType(RecordType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RecordType>();
                rep.Insert(t);
            }
        }

        public void DeleteRecordType(int recordTypeId)
        {
            var t = GetRecordType(recordTypeId);
            DeleteRecordType(t);
        }

        public void DeleteRecordType(RecordType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RecordType>();
                rep.Delete(t);
            }
        }             
        public IEnumerable<RecordType> GetRecordTypes()
        {
            IEnumerable<RecordType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RecordType>();
                t = rep.Get();
            }
            return t;
        }
        public RecordType GetRecordType(int recordTypeId)
        {
            RecordType t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RecordType>();
                t = rep.GetById(recordTypeId);
            }
            return t;
        }

        public void UpdateRecordType(RecordType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RecordType>();
                rep.Update(t);
            }
        }
    }
}
