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


namespace tjc.Modules.ThreatReport.Components
{
    class AttachmentController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection

        public void CreateAttachment(Attachment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attachment>();
                rep.Insert(t);
            }
        }

        public void DeleteAttachment(int attachmentId)
        {
            var t = GetAttachment(attachmentId);
            DeleteAttachment(t);
        }

        public void DeleteAttachment(Attachment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attachment>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Attachment> GetAttachments(int id)
        {
            IEnumerable<Attachment> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Find("Where IncidentID = @0", id);
            }
            return t;
        }

        public Attachment GetAttachment(int attachmentId)
        {
            Attachment t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.GetById(attachmentId);
            }
            return t;
        }

        public void UpdateAttachment(Attachment t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Attachment>();
                rep.Update(t);
            }
        }

    }
}
